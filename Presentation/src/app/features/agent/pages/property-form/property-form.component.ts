import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PropertyService } from '../../../../core/services/property.service';
import { AgentService } from '../../../../core/services/agent.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { ThemeService } from '../../../../core/services/theme.service';
import { ImageUploadService } from '../../../../core/services/image-upload.service';
import { AgentSidebarComponent } from '../../components/agent-sidebar/agent-sidebar.component';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-property-form',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, ReactiveFormsModule, AgentSidebarComponent],
  templateUrl: './property-form.component.html',
  styleUrl: './property-form.component.scss'
})
export class PropertyFormComponent implements OnInit {
  propertyForm!: FormGroup;
  loading: boolean = false;
  isEditMode: boolean = false;
  propertyId: string | null = null;
  agentId: string | null = null;
  currentStep: number = 1;
  totalSteps: number = 4;
  
  // Image management
  imageUrls: string[] = [];
  imageFiles: File[] = [];
  isDragging: boolean = false;
  uploadingImages: boolean = false;

  propertyTypes = [
    { value: 'House', label: 'Moradia' },
    { value: 'Apartment', label: 'Apartamento' },
    { value: 'Land', label: 'Terreno' },
    { value: 'Commercial', label: 'Comercial' },
    { value: 'Office', label: 'Escritório' },
    { value: 'Warehouse', label: 'Armazém' }
  ];

  transactionTypes = [
    { value: 'Sale', label: 'Venda' },
    { value: 'Rent', label: 'Arrendamento' }
  ];

  constructor(
    private fb: FormBuilder,
    private propertyService: PropertyService,
    private agentService: AgentService,
    private authService: AuthService,
    private toastService: ToastService,
    private imageUploadService: ImageUploadService,
    private router: Router,
    private route: ActivatedRoute,
    private http: HttpClient,
    public themeService: ThemeService
  ) {
    this.initializeForm();
  }

  ngOnInit(): void {
    const currentUser = this.authService.getCurrentUser();
    if (currentUser) {
      this.loadAgentProfile(currentUser.id);
    }

    // Check if we're in edit mode
    this.route.params.subscribe(params => {
      if (params['id']) {
        this.isEditMode = true;
        this.propertyId = params['id'];
        this.loadProperty(params['id']);
      }
    });
  }

  loadAgentProfile(userId: string): void {
    this.agentService.getByUserId(userId).subscribe({
      next: (agent: any) => {
        this.agentId = agent.id;
      },
      error: (error) => {
        console.error('Error loading agent profile:', error);
        this.toastService.error('Erro ao carregar perfil do agente');
      }
    });
  }

  initializeForm(): void {
    this.propertyForm = this.fb.group({
      // Step 1: Basic Information
      title: ['', [Validators.required, Validators.minLength(10)]],
      description: ['', [Validators.required, Validators.minLength(50)]],
      propertyType: ['', Validators.required],
      transactionType: ['', Validators.required],
      price: ['', [Validators.required, Validators.min(1)]],
      
      // Step 2: Property Details
      area: ['', [Validators.required, Validators.min(1)]],
      bedrooms: ['', [Validators.required, Validators.min(0)]],
      bathrooms: ['', [Validators.required, Validators.min(0)]],
      floors: [1],
      parkingSpaces: [0],
      hasGarage: [false],
      hasGarden: [false],
      hasPool: [false],
      hasElevator: [false],
      
      // Step 3: Location
      street: ['', Validators.required],
      municipality: ['', Validators.required],
      district: ['', Validators.required],
      postalCode: ['', [Validators.required, Validators.pattern(/^\d{4}-\d{3}$/)]],
      
      // Additional
      energyCertificate: [''],
      constructionYear: ['']
    });
  }

  loadProperty(id: string): void {
    this.loading = true;
    this.http.get<any>(`${environment.apiUrl}/properties/${id}`).subscribe({
      next: (response) => {
        // Check if response is a Result object or direct property
        const property = response.value || response;
        
        console.log('Loaded property for edit:', property); // Debug log
        
        // Helper function to get value with fallback for PascalCase/camelCase
        const getValue = (obj: any, ...keys: string[]) => {
          for (const key of keys) {
            if (obj && obj[key] !== undefined && obj[key] !== null) {
              return obj[key];
            }
          }
          return null;
        };
        
        // Extract features if they exist
        const features = getValue(property, 'features', 'Features') || [];
        const hasGarage = features.includes('Garagem') || getValue(property, 'hasGarage', 'HasGarage') || false;
        const hasGarden = features.includes('Jardim') || getValue(property, 'hasGarden', 'HasGarden') || false;
        const hasPool = features.includes('Piscina') || getValue(property, 'hasPool', 'HasPool') || false;
        const hasElevator = features.includes('Elevador') || getValue(property, 'hasElevator', 'HasElevator') || false;
        
        this.propertyForm.patchValue({
          title: getValue(property, 'title', 'Title') || '',
          description: getValue(property, 'description', 'Description') || '',
          propertyType: getValue(property, 'propertyType', 'PropertyType', 'type', 'Type') || '',
          transactionType: getValue(property, 'transactionType', 'TransactionType') || '',
          price: getValue(property, 'price', 'Price') || 0,
          area: getValue(property, 'area', 'Area', 'size', 'Size') || 0,
          bedrooms: getValue(property, 'bedrooms', 'Bedrooms') || 0,
          bathrooms: getValue(property, 'bathrooms', 'Bathrooms') || 0,
          floors: getValue(property, 'floors', 'Floors') || 1,
          parkingSpaces: getValue(property, 'parkingSpaces', 'ParkingSpaces') || 0,
          hasGarage: hasGarage,
          hasGarden: hasGarden,
          hasPool: hasPool,
          hasElevator: hasElevator,
          street: getValue(property, 'street', 'Street') || getValue(property.address, 'street', 'Street') || '',
          municipality: getValue(property, 'municipality', 'Municipality') || getValue(property.address, 'municipality', 'Municipality') || '',
          district: getValue(property, 'district', 'District') || getValue(property.address, 'district', 'District') || '',
          postalCode: getValue(property, 'postalCode', 'PostalCode') || getValue(property.address, 'postalCode', 'PostalCode') || '',
          energyCertificate: getValue(property, 'energyCertificate', 'EnergyCertificate') || '',
          constructionYear: getValue(property, 'constructionYear', 'ConstructionYear', 'yearBuilt', 'YearBuilt') || ''
        });
        
        // Load existing images
        if (property.images && property.images.length > 0) {
          this.imageUrls = property.images
            .sort((a: any, b: any) => (a.displayOrder || 0) - (b.displayOrder || 0))
            .map((img: any) => img.url || img.imageUrl);
        }
        
        console.log('Form values after load:', this.propertyForm.value); // Debug log
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading property:', error);
        this.toastService.error('Erro ao carregar imóvel');
        this.loading = false;
        this.router.navigate(['/agent/properties']);
      }
    });
  }

  nextStep(): void {
    if (this.currentStep < this.totalSteps) {
      if (this.validateCurrentStep()) {
        this.currentStep++;
      }
    }
  }

  previousStep(): void {
    if (this.currentStep > 1) {
      this.currentStep--;
    }
  }

  validateCurrentStep(): boolean {
    let fieldsToValidate: string[] = [];

    switch (this.currentStep) {
      case 1:
        fieldsToValidate = ['title', 'description', 'propertyType', 'transactionType', 'price'];
        break;
      case 2:
        fieldsToValidate = ['area', 'bedrooms', 'bathrooms'];
        break;
      case 3:
        fieldsToValidate = ['street', 'municipality', 'district', 'postalCode'];
        break;
      case 4:
        // Images are optional but can be validated here if needed
        break;
    }

    for (const field of fieldsToValidate) {
      const control = this.propertyForm.get(field);
      if (control && control.invalid) {
        control.markAsTouched();
        this.toastService.error(`Por favor, preencha todos os campos obrigatórios`);
        return false;
      }
    }

    return true;
  }

  onSubmit(): void {
    if (this.propertyForm.invalid) {
      this.toastService.error('Por favor, preencha todos os campos obrigatórios');
      Object.keys(this.propertyForm.controls).forEach(key => {
        this.propertyForm.get(key)?.markAsTouched();
      });
      return;
    }

    if (!this.agentId) {
      this.toastService.error('Erro ao identificar agente');
      return;
    }

    this.loading = true;
    
    const formValue = this.propertyForm.value;
    const formData = new FormData();
    
    // Add basic fields
    formData.append('title', formValue.title);
    formData.append('description', formValue.description);
    formData.append('price', formValue.price.toString());
    formData.append('status', '0'); // Available
    formData.append('size', formValue.area.toString());
    formData.append('bedrooms', formValue.bedrooms.toString());
    formData.append('bathrooms', formValue.bathrooms.toString());
    
    // Property type
    const propertyTypeValue = formValue.propertyType === 'House' ? 0 : 
          formValue.propertyType === 'Apartment' ? 1 : 
          formValue.propertyType === 'Land' ? 2 : 
          formValue.propertyType === 'Commercial' ? 3 : 
          formValue.propertyType === 'Office' ? 4 : 5;
    formData.append('type', propertyTypeValue.toString());
    formData.append('transactionType', formValue.transactionType === 'Sale' ? '0' : '1');
    
    // Address
    formData.append('street', formValue.street);
    formData.append('number', '0');
    formData.append('parish', formValue.municipality);
    formData.append('municipality', formValue.municipality);
    formData.append('district', formValue.district);
    formData.append('postalCode', formValue.postalCode);
    
    // Optional fields
    if (formValue.area) formData.append('grossArea', formValue.area.toString());
    if (formValue.floors) formData.append('floor', formValue.floors.toString());
    if (formValue.parkingSpaces) formData.append('parkingSpaces', formValue.parkingSpaces.toString());
    if (formValue.constructionYear) formData.append('yearBuilt', formValue.constructionYear.toString());
    if (formValue.energyCertificate) formData.append('energyRating', formValue.energyCertificate);
    
    // Boolean fields
    formData.append('hasElevator', formValue.hasElevator.toString());
    formData.append('hasGarage', formValue.hasGarage.toString());
    formData.append('hasPool', formValue.hasPool.toString());
    formData.append('isFurnished', 'false');
    
    if (this.isEditMode && this.propertyId) {
      formData.append('id', this.propertyId);
    } else {
      formData.append('realEstateAgentId', this.agentId!);
    }
    
    // Add images
    this.imageFiles.forEach((file, index) => {
      formData.append('images', file, file.name);
    });
    
    console.log('Sending property with', this.imageFiles.length, 'images');
    
    const request = this.isEditMode && this.propertyId
      ? this.http.put(`${environment.apiUrl}/property/update/${this.propertyId}`, formData)
      : this.http.post(`${environment.apiUrl}/property/create`, formData);

    request.subscribe({
      next: (response) => {
        console.log('Property saved successfully:', response);
        this.toastService.success(this.isEditMode ? 'Imóvel atualizado com sucesso!' : 'Imóvel criado com sucesso!');
        this.router.navigate(['/agent/properties']);
        this.loading = false;
      },
      error: (error) => {
        console.error('Error saving property:', error);
        console.error('Error details:', error.error);
        const errorMsg = error.error?.message || error.message || 'Erro desconhecido';
        this.toastService.error(`${this.isEditMode ? 'Erro ao atualizar' : 'Erro ao criar'} imóvel: ${errorMsg}`);
        this.loading = false;
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/agent/properties']);
  }

  getProgressPercentage(): number {
    return (this.currentStep / this.totalSteps) * 100;
  }

  // Image management methods
  removeImage(index: number): void {
    this.imageUrls.splice(index, 1);
    this.imageFiles.splice(index, 1);
    this.toastService.success('Imagem removida!');
  }

  // File upload methods
  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      this.handleFiles(input.files);
    }
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;

    if (event.dataTransfer?.files) {
      this.handleFiles(event.dataTransfer.files);
    }
  }

  handleFiles(files: FileList): void {
    const maxSize = 5 * 1024 * 1024; // 5MB
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];

    for (let i = 0; i < files.length; i++) {
      const file = files[i];

      // Validate file type
      if (!allowedTypes.includes(file.type)) {
        this.toastService.error(`${file.name}: Tipo de arquivo não suportado. Use JPG, PNG, GIF ou WebP.`);
        continue;
      }

      // Validate file size
      if (file.size > maxSize) {
        this.toastService.error(`${file.name}: Arquivo muito grande. Máximo 5MB.`);
        continue;
      }

      // Add file to list and create preview
      this.imageFiles.push(file);
      this.createPreview(file);
      this.toastService.success(`${file.name} adicionado!`);
    }
  }

  createPreview(file: File): void {
    const reader = new FileReader();
    reader.onload = (e) => {
      const preview = e.target?.result as string;
      this.imageUrls.push(preview);
    };
    reader.readAsDataURL(file);
  }

  moveImageUp(index: number): void {
    if (index > 0) {
      // Swap URLs
      const tempUrl = this.imageUrls[index];
      this.imageUrls[index] = this.imageUrls[index - 1];
      this.imageUrls[index - 1] = tempUrl;
      
      // Swap Files
      const tempFile = this.imageFiles[index];
      this.imageFiles[index] = this.imageFiles[index - 1];
      this.imageFiles[index - 1] = tempFile;
    }
  }

  moveImageDown(index: number): void {
    if (index < this.imageUrls.length - 1) {
      // Swap URLs
      const tempUrl = this.imageUrls[index];
      this.imageUrls[index] = this.imageUrls[index + 1];
      this.imageUrls[index + 1] = tempUrl;
      
      // Swap Files
      const tempFile = this.imageFiles[index];
      this.imageFiles[index] = this.imageFiles[index + 1];
      this.imageFiles[index + 1] = tempFile;
    }
  }
}

