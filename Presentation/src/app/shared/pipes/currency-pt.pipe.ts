import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'currencyPt',
  standalone: true
})
export class CurrencyPtPipe implements PipeTransform {
  transform(value: number): string {
    if (value == null) return '€0';
    return `€${value.toLocaleString('pt-PT', { minimumFractionDigits: 0, maximumFractionDigits: 0 })}`;
  }
}

