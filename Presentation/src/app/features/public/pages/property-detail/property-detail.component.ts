import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PropertyService } from '../../../../core/services/property.service';
import { CommentService } from '../../../../core/services/comment.service';
import { ProposalService } from '../../../../core/services/proposal.service';
import { FavoriteService } from '../../../../core/services/favorite.service';
import { ClientService } from '../../../../core/services/client.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { Property } from '../../../../core/models/property.model';
import { Comment, PropertyRating, CreateCommentRequest } from '../../../../core/models/comment.model';
import { CreateProposalRequest, ProposalType } from '../../../../core/models/proposal.model';
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
  proposalNotes: string = '';
  
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
    private authService: AuthService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    const propertyId = this.route.snapshot.paramMap.get('id');
    if (propertyId) {
      this.loadPropertyDetails(propertyId);
      this.loadComments(propertyId);
      this.loadRating(propertyId);
      
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

    this.clientService.getByUserId(currentUser.id).subscribe({
      next: (client: any) => {
        this.clientId = client?.id || null;
        if (this.clientId && this.property) {
          this.checkIfFavorited();
        }
      },
      error: (error: any) => {
        console.error('Error loading client profile:', error);
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
      next: (property) => {
        this.property = property;
        this.proposalValue = property.price;
        
        // Initialize images array if undefined
        if (!this.property.images) {
          this.property.images = [];
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

    if (this.proposalValue <= 0) {
      this.toastService.warning('Insira um valor válido');
      return;
    }

    const proposalRequest: CreateProposalRequest = {
      propertyId: this.property!.id,
      clientId: currentUser.id,
      proposedValue: this.proposalValue,
      type: this.property!.transactionType === 'Sale' ? ProposalType.Purchase : ProposalType.Rent,
      additionalNotes: this.proposalNotes
    };

    this.proposalService.create(proposalRequest).subscribe((result: any) => {
      if (result.isSuccess) {
        this.toastService.success('Proposta enviada com sucesso!');
        this.showProposalForm = false;
        this.proposalValue = this.property!.price;
        this.proposalNotes = '';
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

    if (!this.clientId) {
      this.toastService.warning('Perfil de cliente não encontrado');
      return;
    }

    if (!this.property) return;

    this.favoriteLoading = true;

    if (this.isFavorited) {
      this.favoriteService.removeFavorite(this.clientId, this.property.id).subscribe({
        next: (result: any) => {
          this.favoriteLoading = false;
          if (result.isSuccess) {
            this.isFavorited = false;
            this.toastService.success('Removido dos favoritos');
          } else {
            this.toastService.error('Erro ao remover dos favoritos');
          }
        },
        error: (error: any) => {
          this.favoriteLoading = false;
          console.error('Error removing favorite:', error);
          this.toastService.error('Erro ao remover dos favoritos');
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
            this.toastService.error('Erro ao adicionar aos favoritos');
          }
        },
        error: (error: any) => {
          this.favoriteLoading = false;
          console.error('Error adding favorite:', error);
          this.toastService.error('Erro ao adicionar aos favoritos');
        }
      });
    }
  }
}

