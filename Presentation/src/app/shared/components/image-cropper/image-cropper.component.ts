import { Component, EventEmitter, Input, Output, ViewChild, ElementRef, AfterViewInit, OnChanges, SimpleChanges, OnDestroy, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-image-cropper',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './image-cropper.component.html',
  styleUrl: './image-cropper.component.scss'
})
export class ImageCropperComponent implements AfterViewInit, OnChanges, OnDestroy {
  @Input() imageFile: File | null = null;
  @Output() croppedImage = new EventEmitter<File>();
  @Output() cancel = new EventEmitter<void>();

  @ViewChild('canvas', { static: false }) canvasRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('image', { static: false }) imageRef!: ElementRef<HTMLImageElement>;

  imageSrc: string = '';
  scale: number = 1;
  position: { x: number; y: number } = { x: 0, y: 0 };
  isDragging: boolean = false;
  dragStart: { x: number; y: number } = { x: 0, y: 0 };
  cropSize: number = 300; // Tamanho do crop circular (display)
  outputSize: number = 400; // Tamanho de saída para melhor qualidade

  ngAfterViewInit(): void {
    if (this.imageFile) {
      this.loadImage();
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['imageFile'] && this.imageFile) {
      this.loadImage();
    }
  }

  loadImage(): void {
    if (!this.imageFile) return;

    const reader = new FileReader();
    reader.onload = (e: any) => {
      this.imageSrc = e.target.result;
      setTimeout(() => {
        this.initializeCrop();
      }, 100);
    };
    reader.readAsDataURL(this.imageFile);
  }

  initializeCrop(): void {
    if (!this.imageRef?.nativeElement) return;

    const img = this.imageRef.nativeElement;
    if (img.complete) {
      this.setupCrop(img);
    } else {
      img.onload = () => {
        this.setupCrop(img);
      };
    }
  }

  private setupCrop(img: HTMLImageElement): void {
    // Usar naturalWidth/naturalHeight para cálculos precisos
    const imgWidth = img.naturalWidth || img.width;
    const imgHeight = img.naturalHeight || img.height;
    const imgAspect = imgWidth / imgHeight;
    const cropAspect = 1; // Circular é 1:1

    // Calcular escala mínima para cobrir o círculo
    let minScale: number;
    if (imgAspect > cropAspect) {
      // Imagem mais larga - escala baseada na altura para preencher
      minScale = this.cropSize / imgHeight;
    } else {
      // Imagem mais alta ou quadrada - escala baseada na largura para preencher
      minScale = this.cropSize / imgWidth;
    }

    // Usar uma escala significativamente maior para garantir muito espaço de movimento
    // Para imagens pequenas, usar 2.5x, para imagens grandes usar 2x
    // Isso garante que sempre haja espaço para mover a imagem
    const scaleMultiplier = imgWidth < 500 || imgHeight < 500 ? 2.5 : 2.0;
    this.scale = minScale * scaleMultiplier;

    // Centralizar a imagem
    // Posição relativa ao centro (0,0 = centro)
    this.position = {
      x: 0,
      y: 0
    };

    this.updatePreview();
  }

  onMouseDown(event: MouseEvent): void {
    if (event.button !== 0) return; // Apenas botão esquerdo
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = true;
    
    // Guardar referência do container
    this.cropContainerRef = (event.currentTarget as HTMLElement).closest('.crop-container') as HTMLElement;
    if (!this.cropContainerRef) return;
    
    const rect = this.cropContainerRef.getBoundingClientRect();
    const centerX = rect.left + rect.width / 2;
    const centerY = rect.top + rect.height / 2;
    
    // Calcular posição do mouse relativa ao centro
    const mouseX = event.clientX - centerX;
    const mouseY = event.clientY - centerY;
    
    // Guardar a diferença entre a posição do mouse e a posição atual da imagem
    // Isso permite arrastar suavemente a partir de qualquer ponto
    this.dragStart = {
      x: mouseX - this.position.x,
      y: mouseY - this.position.y
    };
    
    // Adicionar listeners globais para permitir arrastar mesmo quando o mouse sai do elemento
    document.addEventListener('mousemove', this.onGlobalMouseMove, { passive: false });
    document.addEventListener('mouseup', this.onGlobalMouseUp);
  }

  @HostListener('mousemove', ['$event'])
  onMouseMove(event: MouseEvent): void {
    if (!this.isDragging) return;
    this.updatePosition(event);
  }

  private onGlobalMouseMove = (event: MouseEvent): void => {
    if (!this.isDragging) return;
    this.updatePosition(event);
  };

  private cropContainerRef: HTMLElement | null = null;

  private updatePosition(event: MouseEvent): void {
    if (!this.cropContainerRef) {
      this.cropContainerRef = document.querySelector('.crop-container') as HTMLElement;
    }
    
    if (!this.cropContainerRef) return;

    event.preventDefault();
    event.stopPropagation();

    const rect = this.cropContainerRef.getBoundingClientRect();
    const centerX = rect.left + rect.width / 2;
    const centerY = rect.top + rect.height / 2;
    
    // Calcular nova posição do mouse relativa ao centro
    const mouseX = event.clientX - centerX;
    const mouseY = event.clientY - centerY;

    // Calcular nova posição da imagem subtraindo o offset inicial
    this.position = {
      x: mouseX - this.dragStart.x,
      y: mouseY - this.dragStart.y
    };

    // Limitar movimento dentro dos bounds
    this.constrainPosition();
    this.updatePreview();
  }

  @HostListener('mouseup')
  onMouseUp(): void {
    this.stopDragging();
  }

  private onGlobalMouseUp = (): void => {
    this.stopDragging();
  };

  private stopDragging(): void {
    if (this.isDragging) {
      this.isDragging = false;
      // Remover listeners globais
      document.removeEventListener('mousemove', this.onGlobalMouseMove);
      document.removeEventListener('mouseup', this.onGlobalMouseUp);
    }
  }

  ngOnDestroy(): void {
    // Limpar listeners ao destruir o componente
    this.stopDragging();
  }

  onWheel(event: WheelEvent): void {
    event.preventDefault();
    event.stopPropagation();
    
    // Zoom mais suave e preciso
    const zoomSpeed = 0.05;
    const delta = event.deltaY > 0 ? -zoomSpeed : zoomSpeed;
    // Aumentar limite máximo de zoom para 5x para dar mais espaço de movimento
    const newScale = Math.max(0.5, Math.min(5, this.scale + delta));
    
    // Ajustar posição ao fazer zoom para manter o centro visual
    if (newScale !== this.scale) {
      const scaleChange = newScale / this.scale;
      this.scale = newScale;
      
      // Ajustar posição proporcionalmente ao zoom
      this.position.x *= scaleChange;
      this.position.y *= scaleChange;
      
      this.constrainPosition();
      this.updatePreview();
    }
  }

  constrainPosition(): void {
    if (!this.imageRef?.nativeElement) return;

    const img = this.imageRef.nativeElement;
    const imgWidth = img.naturalWidth || img.width;
    const imgHeight = img.naturalHeight || img.height;
    
    // Dimensões da imagem escalada
    const scaledWidth = imgWidth * this.scale;
    const scaledHeight = imgHeight * this.scale;
    
    // Raio do círculo de crop
    const cropRadius = this.cropSize / 2;
    
    // Calcular quanto a imagem pode se mover em cada direção
    // A imagem deve sempre cobrir o círculo completamente
    // Limite máximo: quando a borda da imagem toca a borda do círculo
    
    // Para X: a imagem pode se mover até que sua borda esquerda/direita toque o círculo
    // Adicionar uma margem de segurança para garantir que sempre cubra
    const maxMoveX = Math.max(10, (scaledWidth / 2) - cropRadius);
    const maxMoveY = Math.max(10, (scaledHeight / 2) - cropRadius);
    
    // Aplicar limites (simétricos em ambas as direções)
    // Garantir pelo menos 10px de movimento em cada direção
    this.position.x = Math.max(-maxMoveX, Math.min(maxMoveX, this.position.x));
    this.position.y = Math.max(-maxMoveY, Math.min(maxMoveY, this.position.y));
  }

  updatePreview(): void {
    // Preview é atualizado via CSS transform
  }

  zoomIn(): void {
    const newScale = Math.min(5, this.scale + 0.1);
    if (newScale !== this.scale) {
      const scaleChange = newScale / this.scale;
      this.scale = newScale;
      this.position.x *= scaleChange;
      this.position.y *= scaleChange;
      this.constrainPosition();
      this.updatePreview();
    }
  }

  zoomOut(): void {
    const newScale = Math.max(0.5, this.scale - 0.1);
    if (newScale !== this.scale) {
      const scaleChange = newScale / this.scale;
      this.scale = newScale;
      this.position.x *= scaleChange;
      this.position.y *= scaleChange;
      this.constrainPosition();
      this.updatePreview();
    }
  }

  onZoomChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    const newScale = parseFloat(input.value) / 100;
    if (newScale !== this.scale) {
      const scaleChange = newScale / this.scale;
      this.scale = newScale;
      this.position.x *= scaleChange;
      this.position.y *= scaleChange;
      this.constrainPosition();
      this.updatePreview();
    }
  }

  reset(): void {
    this.initializeCrop();
  }

  crop(): void {
    if (!this.canvasRef?.nativeElement || !this.imageRef?.nativeElement) return;

    const canvas = this.canvasRef.nativeElement;
    const ctx = canvas.getContext('2d');
    const img = this.imageRef.nativeElement;

    if (!ctx) return;

    // Wait for image to load if not ready
    if (!img.complete) {
      img.onload = () => this.performCrop(canvas, ctx, img);
      return;
    }

    this.performCrop(canvas, ctx, img);
  }

  private performCrop(canvas: HTMLCanvasElement, ctx: CanvasRenderingContext2D, img: HTMLImageElement): void {
    // Usar tamanho maior para melhor qualidade
    const outputSize = this.outputSize;
    canvas.width = outputSize;
    canvas.height = outputSize;

    // Limpar canvas com fundo transparente (melhor para PNG)
    ctx.clearRect(0, 0, outputSize, outputSize);

    // Criar clipping path circular
    ctx.save();
    ctx.beginPath();
    ctx.arc(outputSize / 2, outputSize / 2, outputSize / 2, 0, 2 * Math.PI);
    ctx.clip();

    // Calcular escala proporcional ao tamanho de saída
    // O scale atual é relativo ao cropSize (300px), precisamos escalar para outputSize (400px)
    const scaleRatio = outputSize / this.cropSize;
    
    // Dimensões da imagem escaladas no tamanho de saída
    const scaledWidth = img.naturalWidth * this.scale * scaleRatio;
    const scaledHeight = img.naturalHeight * this.scale * scaleRatio;
    
    // Calcular posição de desenho
    // this.position é relativo ao centro (0,0 = centro do crop de 300px)
    // Precisamos converter para o tamanho de saída (400px) e calcular a posição absoluta
    const centerX = outputSize / 2;
    const centerY = outputSize / 2;
    
    // Converter offset do display (300px) para output (400px)
    const offsetX = this.position.x * scaleRatio;
    const offsetY = this.position.y * scaleRatio;
    
    // Calcular posição absoluta: centro - metade da imagem + offset
    const drawX = centerX - (scaledWidth / 2) + offsetX;
    const drawY = centerY - (scaledHeight / 2) + offsetY;

    // Desenhar imagem recortada com alta qualidade
    ctx.imageSmoothingEnabled = true;
    ctx.imageSmoothingQuality = 'high';
    
    // Usar naturalWidth/naturalHeight para melhor qualidade
    ctx.drawImage(
      img,
      0, 0, img.naturalWidth, img.naturalHeight, // Source rectangle (imagem completa)
      drawX, drawY, scaledWidth, scaledHeight     // Destination rectangle (no canvas)
    );
    
    ctx.restore();

    // Converter para blob e depois para File com alta qualidade
    canvas.toBlob((blob) => {
      if (blob) {
        // Usar o nome original se possível, senão usar um nome padrão
        const originalName = this.imageFile?.name || 'profile-image';
        const nameWithoutExt = originalName.replace(/\.[^/.]+$/, '');
        const file = new File([blob], `${nameWithoutExt}-cropped.png`, {
          type: 'image/png',
          lastModified: Date.now()
        });
        this.croppedImage.emit(file);
      } else {
        console.error('Erro ao criar blob da imagem recortada');
      }
    }, 'image/png', 1.0); // Qualidade máxima para PNG
  }

  cancelCrop(): void {
    this.cancel.emit();
  }
}
