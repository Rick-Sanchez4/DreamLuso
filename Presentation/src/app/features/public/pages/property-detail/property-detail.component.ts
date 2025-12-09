import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PropertyService } from '../../../../core/services/property.service';
import { CommentService } from '../../../../core/services/comment.service';
import { ProposalService } from '../../../../core/services/proposal.service';
import { FavoriteService } from '../../../../core/services/favorite.service';
import { ClientService } from '../../../../core/services/client.service';
import { VisitService } from '../../../../core/services/visit.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { Property } from '../../../../core/models/property.model';
import { Comment, PropertyRating, CreateCommentRequest } from '../../../../core/models/comment.model';
import { CreateProposalRequest, ProposalType } from '../../../../core/models/proposal.model';
import { ScheduleVisitRequest, TimeSlot } from '../../../../core/models/visit.model';
import { NavbarComponent } from '../../../../shared/components/navbar/navbar.component';
import { FooterComponent } from '../../../../shared/components/footer/footer.component';
import { RatingStarsComponent } from '../../../../shared/components/rating-stars/rating-stars.component';
import { CurrencyPtPipe } from '../../../../shared/pipes/currency-pt.pipe';

@Component({
  selector: 'app-property-detail',
  standalone: true,
  imports: [CommonModule, DatePipe, FormsModule, RouterModule, NavbarComponent, FooterComponent, RatingStarsComponent, CurrencyPtPipe],
  templateUrl: './property-detail.component.html',
  styleUrl: './property-detail.component.scss'
})
export class PropertyDetailComponent implements OnInit {
  property: Property | null = null;
  comments: Comment[] = [];
  rating: PropertyRating | null = null;
  loading: boolean = true;
  
  // Comment Form
  newComment: string = '';
  newRating: number = 0;
  
  // Proposal Form
  showProposalForm: boolean = false;
  proposalValue: number = 0;
  proposalValueFormatted: string = '';
  proposalNotes: string = '';
  
  // Visit Form
  showVisitForm: boolean = false;
  visitDate: string = '';
  visitTimeSlot: number = TimeSlot.Morning_9AM_11AM;
  visitNotes: string = '';
  visitLoading: boolean = false;
  minVisitDate: string = '';
  availableTimeSlots: Set<number> = new Set(); // Horários disponíveis para a data selecionada
  loadingTimeSlots: boolean = false;
  
  // Favorites
  clientId: string | null = null;
  isFavorited: boolean = false;
  favoriteLoading: boolean = false;
  
  currentImageIndex: number = 0;

  constructor(
    private route: ActivatedRoute,
    private propertyService: PropertyService,
    private commentService: CommentService,
    private proposalService: ProposalService,
    private favoriteService: FavoriteService,
    private clientService: ClientService,
    private visitService: VisitService,
    private authService: AuthService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    const propertyId = this.route.snapshot.paramMap.get('id');
    if (propertyId) {
      this.loadPropertyDetails(propertyId);
      this.loadComments(propertyId);
      this.loadRating(propertyId);
      
      // Initialize minimum visit date
      const today = new Date();
      this.minVisitDate = today.toISOString().split('T')[0];
      
      // Load client profile if authenticated
      const currentUser = this.authService.getCurrentUser();
      if (currentUser) {
        this.loadClientProfile();
      }
    }
  }

  loadClientProfile(): void {
    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) return;

    // Only load client profile if user is a Client
    if (currentUser.role !== 'Client') {
      return;
    }

    this.clientService.getByUserId(currentUser.id).subscribe({
      next: (client: any) => {
        this.clientId = client?.id || null;
        if (this.clientId && this.property) {
          this.checkIfFavorited();
        }
      },
      error: (error: any) => {
        // Log error but don't show to user - profile might be created automatically
        if (error.status !== 404) {
          console.error('Error loading client profile:', error);
        } else {
          // 404 means client profile doesn't exist yet - it will be created automatically by backend
          console.log('Client profile not found, will be created automatically when needed');
        }
      }
    });
  }

  checkIfFavorited(): void {
    if (!this.clientId || !this.property) return;

    this.favoriteService.getFavorites(this.clientId).subscribe({
      next: (result: any) => {
        if (result.isSuccess && result.value) {
          this.isFavorited = result.value.some((p: any) => p.id === this.property?.id);
        }
      },
      error: (error: any) => {
        console.error('Error checking favorites:', error);
      }
    });
  }

  loadPropertyDetails(id: string): void {
    this.propertyService.getById(id).subscribe({
      next: (property: Property) => {
        this.property = property;
        this.proposalValue = property.price;
        
        // Initialize images array if undefined
        if (!this.property?.images) {
          if (this.property) {
            this.property.images = [];
          }
        }
        
        this.loading = false;
        
        // Check if favorited after property is loaded
        if (this.clientId) {
          this.checkIfFavorited();
        }
      },
      error: (error: any) => {
        console.error('Error loading property:', error);
        this.loading = false;
        this.toastService.error('Erro ao carregar imóvel');
      }
    });
  }

  loadComments(propertyId: string): void {
    this.commentService.getPropertyComments(propertyId).subscribe((result: any) => {
      if (result.isSuccess && result.value) {
        this.comments = result.value;
      }
    });
  }

  loadRating(propertyId: string): void {
    this.commentService.getPropertyRating(propertyId).subscribe((result: any) => {
      if (result.isSuccess && result.value) {
        this.rating = result.value;
      }
    });
  }

  submitComment(): void {
    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) {
      this.toastService.warning('Faça login para comentar');
      return;
    }

    if (!this.newComment.trim()) {
      this.toastService.warning('Escreva um comentário');
      return;
    }

    const commentRequest: CreateCommentRequest = {
      propertyId: this.property!.id,
      userId: currentUser.id,
      message: this.newComment,
      rating: this.newRating > 0 ? this.newRating : undefined
    };

    this.commentService.create(commentRequest).subscribe((result: any) => {
      if (result.isSuccess) {
        this.toastService.success('Comentário adicionado com sucesso!');
        this.newComment = '';
        this.newRating = 0;
        this.loadComments(this.property!.id);
        this.loadRating(this.property!.id);
      }
    });
  }

  submitProposal(): void {
    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) {
      this.toastService.warning('Faça login para fazer uma proposta');
      return;
    }

    // Verificar se o usuário é um cliente
    if (currentUser.role !== 'Client') {
      this.toastService.warning('Apenas clientes podem fazer propostas');
      return;
    }

    if (this.proposalValue <= 0) {
      this.toastService.warning('Insira um valor válido');
      return;
    }

    if (!this.property) {
      this.toastService.error('Erro: propriedade não encontrada');
      return;
    }

    // Se não temos clientId, tentar carregar o perfil primeiro
    if (!this.clientId) {
      this.clientService.getByUserId(currentUser.id).subscribe({
        next: (client: any) => {
          this.clientId = client?.id || null;
          if (this.clientId) {
            // Tentar novamente após carregar o clientId
            this.submitProposalWithClientId();
          } else {
            this.toastService.error('Perfil de cliente não encontrado. Por favor, contacte o suporte.');
          }
        },
        error: (error: any) => {
          console.error('Error loading client profile for proposal:', error);
          this.toastService.error('Erro ao carregar perfil de cliente. Por favor, tente novamente.');
        }
      });
      return;
    }

    this.submitProposalWithClientId();
  }

  private submitProposalWithClientId(): void {
    if (!this.clientId || !this.property) {
      this.toastService.error('Erro: dados incompletos para enviar proposta');
      return;
    }

    // Parse the formatted value back to number
    const numericValue = this.parseFormattedValue(this.proposalValueFormatted);

    const proposalRequest: CreateProposalRequest = {
      propertyId: this.property.id,
      clientId: this.clientId,
      proposedValue: numericValue,
      type: this.property.transactionType === 'Sale' ? ProposalType.Purchase : ProposalType.Rent,
      additionalNotes: this.proposalNotes
    };

    console.log('Submitting proposal:', {
      propertyId: proposalRequest.propertyId,
      clientId: proposalRequest.clientId,
      proposedValue: proposalRequest.proposedValue,
      type: proposalRequest.type
    });

    this.proposalService.create(proposalRequest).subscribe({
      next: (result: any) => {
        if (result.isSuccess) {
          this.toastService.success('Proposta enviada com sucesso! Você receberá uma notificação em breve.');
          
          // Esconder o formulário
          this.showProposalForm = false;
          this.proposalValue = this.property!.price;
          this.proposalValueFormatted = this.formatCurrency(this.property!.price);
          this.proposalNotes = '';
          
          // Recarregar a página após um pequeno delay para garantir que a notificação seja criada
          setTimeout(() => {
            window.location.reload();
          }, 1500);
        } else {
          const errorMsg = result.error?.description || result.error?.message || 'Erro ao enviar proposta';
          this.toastService.error(errorMsg);
        }
      },
      error: (error: any) => {
        console.error('Error submitting proposal:', error);
        let errorMsg = 'Erro ao enviar proposta';
        
        if (error.error?.description) {
          errorMsg = error.error.description;
        } else if (error.error?.message) {
          errorMsg = error.error.message;
        } else if (error.message) {
          errorMsg = error.message;
        }

        // Mensagens específicas para erros comuns
        if (error.status === 400) {
          if (error.error?.code === 'NotFound') {
            errorMsg = 'Cliente não encontrado. Por favor, contacte o suporte.';
          } else if (error.error?.code === 'ProposalAlreadyExists') {
            errorMsg = 'Já existe uma proposta pendente para este imóvel.';
          }
        }

        this.toastService.error(errorMsg);
      }
    });
  }

  incrementHelpful(commentId: string): void {
    this.commentService.incrementHelpful(commentId).subscribe((result: any) => {
      if (result.isSuccess) {
        this.loadComments(this.property!.id);
      }
    });
  }

  nextImage(): void {
    if (this.property && this.property.images.length > 0) {
      this.currentImageIndex = (this.currentImageIndex + 1) % this.property.images.length;
    }
  }

  previousImage(): void {
    if (this.property && this.property.images.length > 0) {
      this.currentImageIndex = this.currentImageIndex === 0 
        ? this.property.images.length - 1 
        : this.currentImageIndex - 1;
    }
  }

  get isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }

  getStarCount(star: number): number {
    if (!this.rating) return 0;
    switch (star) {
      case 5: return this.rating.fiveStars;
      case 4: return this.rating.fourStars;
      case 3: return this.rating.threeStars;
      case 2: return this.rating.twoStars;
      case 1: return this.rating.oneStar;
      default: return 0;
    }
  }

  getStarPercentage(star: number): number {
    if (!this.rating || this.rating.totalComments === 0) return 0;
    return (this.getStarCount(star) / this.rating.totalComments) * 100;
  }

  toggleFavorite(): void {
    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) {
      this.toastService.warning('Faça login para adicionar aos favoritos');
      return;
    }

    // Verificar se o usuário é um cliente
    if (currentUser.role !== 'Client') {
      this.toastService.warning('Apenas clientes podem adicionar aos favoritos');
      return;
    }

    if (!this.property) return;

    // Se não temos clientId, tentar carregar o perfil primeiro
    if (!this.clientId) {
      this.clientService.getByUserId(currentUser.id).subscribe({
        next: (client: any) => {
          this.clientId = client?.id || null;
          if (this.clientId) {
            // Tentar novamente após carregar o clientId
            this.toggleFavoriteWithClientId();
          } else {
            this.toastService.error('Perfil de cliente não encontrado. Por favor, contacte o suporte.');
          }
        },
        error: (error: any) => {
          console.error('Error loading client profile for favorite:', error);
          this.toastService.error('Erro ao carregar perfil de cliente. Por favor, tente novamente.');
        }
      });
      return;
    }

    this.toggleFavoriteWithClientId();
  }

  showContactModal: boolean = false;
  showLoginModal: boolean = false;

  contactAgent(): void {
    if (!this.property) return;
    this.showContactModal = true;
  }

  closeContactModal(): void {
    this.showContactModal = false;
  }

  openProposalFromContact(): void {
    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) {
      this.closeContactModal();
      this.showLoginModal = true;
      return;
    }
    
    this.closeContactModal();
    this.showProposalForm = true;
    // Scroll to proposal form
    setTimeout(() => {
      const proposalForm = document.querySelector('.mt-6.pt-6.border-t');
      if (proposalForm) {
        proposalForm.scrollIntoView({ behavior: 'smooth', block: 'start' });
      }
    }, 100);
  }

  openVisitFromContact(): void {
    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) {
      this.closeContactModal();
      this.showLoginModal = true;
      return;
    }
    
    this.closeContactModal();
    this.openVisitForm();
    // Scroll to visit form
    setTimeout(() => {
      const visitForm = document.querySelector('.mt-6.pt-6.border-t');
      if (visitForm) {
        visitForm.scrollIntoView({ behavior: 'smooth', block: 'start' });
      }
    }, 100);
  }

  closeLoginModal(): void {
    this.showLoginModal = false;
  }

  openVisitForm(): void {
    console.log('openVisitForm() called');
    const currentUser = this.authService.getCurrentUser();
    console.log('Current user:', currentUser);
    
    if (!currentUser) {
      console.log('No current user, showing login modal');
      this.showLoginModal = true;
      return;
    }

    // Verificar se o usuário é um cliente
    if (currentUser.role !== 'Client') {
      console.log('User is not a client, role:', currentUser.role);
      this.toastService.warning('Apenas clientes podem agendar visitas');
      return;
    }

    console.log('User is a client, checking clientId:', this.clientId);

    // Verificar se temos o clientId carregado
    if (!this.clientId) {
      console.log('ClientId not found, loading client profile...');
      this.clientService.getByUserId(currentUser.id).subscribe({
        next: (client: any) => {
          console.log('Client profile loaded:', client);
          this.clientId = client?.id || null;
          if (this.clientId) {
            console.log('ClientId set, showing form');
            this.showVisitForm = true;
            // Set default date to tomorrow
            const tomorrow = new Date();
            tomorrow.setDate(tomorrow.getDate() + 1);
            this.visitDate = tomorrow.toISOString().split('T')[0];
            console.log('Visit form should be visible now, showVisitForm:', this.showVisitForm);
          } else {
            console.error('Client profile found but no id');
            this.toastService.error('Perfil de cliente não encontrado. Por favor, contacte o suporte.');
          }
        },
        error: (error: any) => {
          console.error('Error loading client profile for visit:', error);
          this.toastService.error('Erro ao carregar perfil de cliente. Por favor, tente novamente.');
        }
      });
      return;
    }

    console.log('ClientId already exists, showing form directly');
    this.showVisitForm = true;
    // Set minimum date to today
    const today = new Date();
    this.minVisitDate = today.toISOString().split('T')[0];
    // Set default date to tomorrow
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    this.visitDate = tomorrow.toISOString().split('T')[0];
    console.log('Visit form should be visible now, showVisitForm:', this.showVisitForm);
    
    // Carregar horários disponíveis para a data padrão
    if (this.visitDate && this.property?.id && this.property?.realEstateAgentId) {
      this.loadAvailableTimeSlots();
    }
  }

  onVisitDateChange(): void {
    console.log('Visit date changed to:', this.visitDate);
    if (this.visitDate && this.property?.id && this.property?.realEstateAgentId) {
      // Reset timeSlot quando a data mudar
      this.visitTimeSlot = TimeSlot.Morning_9AM_11AM;
      this.loadAvailableTimeSlots();
    } else {
      this.availableTimeSlots.clear();
    }
  }

  loadAvailableTimeSlots(): void {
    if (!this.visitDate || !this.property?.id) {
      console.warn('Cannot load time slots: missing date or property');
      return;
    }

    this.loadingTimeSlots = true;
    console.log('Loading available time slots for property:', this.property.id, 'date:', this.visitDate);

    this.visitService.getAvailableTimeSlots(this.property.id, this.visitDate).subscribe({
      next: (response: any) => {
        console.log('Available time slots response:', response);
        this.availableTimeSlots.clear();
        
        // O backend retorna uma lista de strings como "Morning_9AM_11AM"
        const availableSlots = response.availableSlots || response || [];
        
        availableSlots.forEach((slotName: string) => {
          // Converter nome do slot para número
          const slotNumber = this.getTimeSlotNumberFromName(slotName);
          if (slotNumber !== -1) {
            this.availableTimeSlots.add(slotNumber);
          }
        });
        
        console.log('Available time slots:', Array.from(this.availableTimeSlots));
        
        // Se o horário selecionado não estiver disponível, selecionar o primeiro disponível
        if (!this.availableTimeSlots.has(this.visitTimeSlot) && this.availableTimeSlots.size > 0) {
          const firstAvailable = Array.from(this.availableTimeSlots)[0];
          this.visitTimeSlot = firstAvailable;
          console.log('Selected time slot not available, changed to:', firstAvailable);
        }
        
        this.loadingTimeSlots = false;
      },
      error: (error: any) => {
        console.error('Error loading available time slots:', error);
        // Em caso de erro, permitir todos os horários (fallback)
        this.availableTimeSlots = new Set([0, 1, 2, 3, 4]);
        this.loadingTimeSlots = false;
      }
    });
  }

  getTimeSlotNumberFromName(slotName: string): number {
    const mapping: { [key: string]: number } = {
      'Morning_9AM_11AM': 0,
      'Morning_11AM_1PM': 1,
      'Afternoon_2PM_4PM': 2,
      'Afternoon_4PM_6PM': 3,
      'Evening_6PM_8PM': 4
    };
    return mapping[slotName] ?? -1;
  }

  isTimeSlotAvailable(slotNumber: number): boolean {
    // Se não há horários carregados, permitir todos (ainda carregando ou erro)
    if (this.availableTimeSlots.size === 0) {
      return true;
    }
    return this.availableTimeSlots.has(slotNumber);
  }

  submitVisit(): void {
    console.log('submitVisit() called');
    console.log('Form data:', {
      visitDate: this.visitDate,
      visitTimeSlot: this.visitTimeSlot,
      visitNotes: this.visitNotes,
      clientId: this.clientId,
      property: this.property
    });
    
    const currentUser = this.authService.getCurrentUser();
    console.log('Checking currentUser:', !!currentUser, 'property:', !!this.property, 'clientId:', !!this.clientId);
    
    if (!currentUser || !this.property || !this.clientId) {
      console.error('Missing required data:', { currentUser, property: this.property, clientId: this.clientId });
      this.toastService.error('Dados incompletos para agendar visita');
      return;
    }
    console.log('✓ User, property and clientId validated');

    if (!this.visitDate) {
      console.warn('No visit date selected');
      this.toastService.warning('Selecione uma data para a visita');
      return;
    }
    console.log('✓ Visit date validated:', this.visitDate);

    // Validar que a data não é no passado e não é hoje (deve ser futura)
    const selectedDate = new Date(this.visitDate + 'T00:00:00');
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    console.log('Date validation - selectedDate:', selectedDate, 'today:', today, 'isValid:', selectedDate > today);
    
    if (selectedDate <= today) {
      console.warn('Date is not in the future');
      this.toastService.warning('A data da visita deve ser futura (a partir de amanhã)');
      return;
    }
    console.log('✓ Date is in the future');

    // Validar que o timeSlot está no range válido (0-4)
    console.log('TimeSlot validation - value:', this.visitTimeSlot, 'isValid:', this.visitTimeSlot >= 0 && this.visitTimeSlot <= 4);
    if (this.visitTimeSlot < 0 || this.visitTimeSlot > 4) {
      console.error('Invalid timeSlot');
      this.toastService.error('Horário inválido selecionado');
      return;
    }
    console.log('✓ TimeSlot validated');

    // Validar que temos o realEstateAgentId
    console.log('Checking realEstateAgentId:', this.property.realEstateAgentId);
    if (!this.property.realEstateAgentId) {
      console.error('No realEstateAgentId found');
      this.toastService.error('Agente imobiliário não encontrado para este imóvel');
      return;
    }
    console.log('✓ realEstateAgentId validated:', this.property.realEstateAgentId);

    console.log('All validations passed, preparing request...');
    this.visitLoading = true;

    const visitRequest: ScheduleVisitRequest = {
      propertyId: this.property.id,
      clientId: this.clientId,
      realEstateAgentId: this.property.realEstateAgentId,
      visitDate: this.visitDate, // YYYY-MM-DD format
      timeSlot: this.visitTimeSlot,
      notes: this.visitNotes?.trim() || undefined
    };

    console.log('Scheduling visit with request:', visitRequest);
    console.log('Calling visitService.scheduleVisit()...');

    this.visitService.scheduleVisit(visitRequest).subscribe({
      next: (result: any) => {
        console.log('✅ Visit scheduling SUCCESS - Response received:', result);
        this.visitLoading = false;
        
        // Sempre exibir mensagem de sucesso se chegou aqui (sem erro)
        const visitDateFormatted = new Date(this.visitDate).toLocaleDateString('pt-PT', {
          weekday: 'long',
          year: 'numeric',
          month: 'long',
          day: 'numeric'
        });
        const timeSlotLabel = this.getTimeSlotLabel(this.visitTimeSlot);
        
        const successMessage = `✅ Visita agendada com sucesso para ${visitDateFormatted} às ${timeSlotLabel}!`;
        const infoMessage = `⏳ O agendamento está aguardando confirmação do agente imobiliário. Você receberá uma notificação quando for confirmado.`;
        
        console.log('Preparing to show success message:', successMessage);
        console.log('ToastService available:', !!this.toastService);
        
        // Limpar formulário primeiro
        this.showVisitForm = false;
        this.visitDate = '';
        this.visitTimeSlot = TimeSlot.Morning_9AM_11AM;
        this.visitNotes = '';
        this.availableTimeSlots.clear();
        
        // Exibir mensagem de sucesso
        console.log('Calling toastService.success() NOW');
        this.toastService.success(successMessage, 'Visita Agendada');
        
        // Exibir mensagem informativa sobre confirmação após um pequeno delay
        setTimeout(() => {
          console.log('Calling toastService.info() for confirmation message');
          this.toastService.info(infoMessage, 'Aguardando Confirmação');
        }, 1500);
        
        // Recarregar notificações após um pequeno delay para garantir que foram criadas no backend
        setTimeout(() => {
          // Disparar evento customizado para que o navbar recarregue as notificações
          window.dispatchEvent(new CustomEvent('notifications:refresh'));
        }, 2000);
      },
      error: (error: any) => {
        this.visitLoading = false;
        console.error('Error scheduling visit:', error);
        console.error('Error details:', {
          status: error.status,
          statusText: error.statusText,
          error: error.error,
          url: error.url
        });
        
        let errorMsg = 'Erro ao agendar visita';
        let errorTitle = 'Erro';
        
        if (error.error) {
          // Priorizar description se disponível (backend já retorna mensagem em português)
          if (error.error.description) {
            errorMsg = error.error.description;
            // Se for TimeSlotUnavailable, usar título específico
            if (error.error.code === 'TimeSlotUnavailable') {
              errorTitle = 'Horário Indisponível';
            }
          } else if (error.error.message) {
            errorMsg = error.error.message;
          } else if (error.error.code) {
            switch (error.error.code) {
              case 'TimeSlotUnavailable':
                errorTitle = 'Horário Indisponível';
                errorMsg = 'Este horário não está disponível para esta data. Por favor, escolha outro horário ou outra data.';
                break;
              case 'ClientNotFound':
                errorTitle = 'Cliente Não Encontrado';
                errorMsg = 'Cliente não encontrado. Por favor, contacte o suporte.';
                break;
              case 'AgentNotFound':
                errorTitle = 'Agente Não Encontrado';
                errorMsg = 'Agente não encontrado. Por favor, contacte o suporte.';
                break;
              case 'PropertyNotFound':
                errorTitle = 'Imóvel Não Encontrado';
                errorMsg = 'Imóvel não encontrado. Por favor, recarregue a página.';
                break;
              default:
                errorTitle = 'Erro';
                errorMsg = error.error.code || 'Erro desconhecido ao agendar visita';
            }
          } else if (typeof error.error === 'string') {
            errorMsg = error.error;
          }
        }
        
        // Se for erro 400, pode ser problema de validação
        if (error.status === 400 && !error.error?.code) {
          errorTitle = 'Dados Inválidos';
          errorMsg = 'Dados inválidos. Por favor, verifique a data e o horário selecionados.';
        }
        
        console.log('Displaying error toast:', errorTitle, errorMsg);
        this.toastService.error(errorMsg, errorTitle);
      }
    });
  }

  getTimeSlotLabel(timeSlot: number): string {
    switch (timeSlot) {
      case TimeSlot.Morning_9AM_11AM:
        return 'Manhã (9h - 11h)';
      case TimeSlot.Morning_11AM_1PM:
        return 'Meio-dia (11h - 13h)';
      case TimeSlot.Afternoon_2PM_4PM:
        return 'Tarde (14h - 16h)';
      case TimeSlot.Afternoon_4PM_6PM:
        return 'Final da tarde (16h - 18h)';
      case TimeSlot.Evening_6PM_8PM:
        return 'Noite (18h - 20h)';
      default:
        return 'Manhã (9h - 11h)';
    }
  }

  private toggleFavoriteWithClientId(): void {
    if (!this.clientId || !this.property) return;

    this.favoriteLoading = true;

    if (this.isFavorited) {
      this.favoriteService.removeFavorite(this.clientId, this.property.id).subscribe({
        next: (result: any) => {
          this.favoriteLoading = false;
          if (result.isSuccess) {
            this.isFavorited = false;
            this.toastService.success('Removido dos favoritos');
          } else {
            const errorMsg = result.error?.description || result.error?.message || 'Erro ao remover dos favoritos';
            this.toastService.error(errorMsg);
          }
        },
        error: (error: any) => {
          this.favoriteLoading = false;
          console.error('Error removing favorite:', error);
          let errorMsg = 'Erro ao remover dos favoritos';
          
          if (error.error?.description) {
            errorMsg = error.error.description;
          } else if (error.error?.message) {
            errorMsg = error.error.message;
          } else if (error.status === 400 && error.error?.code === 'PropertyNotFavorited') {
            errorMsg = 'Propriedade não está nos favoritos';
            this.isFavorited = false; // Sincronizar estado
          }
          
          this.toastService.error(errorMsg);
        }
      });
    } else {
      this.favoriteService.addFavorite(this.clientId, this.property.id).subscribe({
        next: (result: any) => {
          this.favoriteLoading = false;
          if (result.isSuccess) {
            this.isFavorited = true;
            this.toastService.success('Adicionado aos favoritos');
          } else {
            const errorMsg = result.error?.description || result.error?.message || 'Erro ao adicionar aos favoritos';
            this.toastService.error(errorMsg);
          }
        },
        error: (error: any) => {
          this.favoriteLoading = false;
          console.error('Error adding favorite:', error);
          let errorMsg = 'Erro ao adicionar aos favoritos';
          
          if (error.error?.description) {
            errorMsg = error.error.description;
          } else if (error.error?.message) {
            errorMsg = error.error.message;
          } else if (error.status === 400 && error.error?.code === 'PropertyAlreadyFavorited') {
            errorMsg = 'Propriedade já está nos favoritos';
            this.isFavorited = true; // Sincronizar estado
          }
          
          this.toastService.error(errorMsg);
        }
      });
    }
  }

  formatCurrency(value: number): string {
    if (!value || value <= 0) return '';
    // Format with dots as thousands separator (Portuguese format)
    return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, '.');
  }

  parseFormattedValue(formattedValue: string): number {
    if (!formattedValue) return 0;
    // Remove all dots and convert to number
    const numericString = formattedValue.replace(/\./g, '');
    const parsed = parseFloat(numericString);
    return isNaN(parsed) ? 0 : parsed;
  }

  onProposalValueInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    let value = input.value;
    
    // Remove all non-digit characters except dots
    value = value.replace(/[^\d.]/g, '');
    
    // Remove multiple consecutive dots
    value = value.replace(/\.{2,}/g, '.');
    
    // Parse to number and format
    const numericValue = this.parseFormattedValue(value);
    this.proposalValue = numericValue;
    
    // Format with dots as thousands separator
    if (numericValue > 0) {
      this.proposalValueFormatted = this.formatCurrency(numericValue);
    } else {
      this.proposalValueFormatted = '';
    }
    
    // Update input value
    input.value = this.proposalValueFormatted;
  }

  onProposalValueFocus(event: Event): void {
    const input = event.target as HTMLInputElement;
    // Show numeric value when focused for easier editing
    if (this.proposalValue > 0) {
      input.value = this.proposalValue.toString();
    }
  }

  onProposalValueBlur(event: Event): void {
    const input = event.target as HTMLInputElement;
    // Format with dots when focus is lost
    if (this.proposalValue > 0) {
      this.proposalValueFormatted = this.formatCurrency(this.proposalValue);
      input.value = this.proposalValueFormatted;
    } else {
      this.proposalValueFormatted = '';
      input.value = '';
    }
  }
}

