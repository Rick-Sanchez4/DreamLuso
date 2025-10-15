import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

// Components
import { NavbarComponent } from './components/navbar/navbar.component';
import { FooterComponent } from './components/footer/footer.component';
import { LoadingSpinnerComponent } from './components/loading-spinner/loading-spinner.component';
import { PropertyCardComponent } from './components/property-card/property-card.component';
import { RatingStarsComponent } from './components/rating-stars/rating-stars.component';

// Pipes
import { CurrencyPtPipe } from './pipes/currency-pt.pipe';
import { DatePtPipe } from './pipes/date-pt.pipe';

const COMPONENTS = [
  NavbarComponent,
  FooterComponent,
  LoadingSpinnerComponent,
  PropertyCardComponent,
  RatingStarsComponent
];

const PIPES = [
  CurrencyPtPipe,
  DatePtPipe
];

@NgModule({
  imports: [
    CommonModule,
    ...COMPONENTS,
    ...PIPES
  ],
  exports: [
    ...COMPONENTS,
    ...PIPES
  ]
})
export class SharedModule { }

