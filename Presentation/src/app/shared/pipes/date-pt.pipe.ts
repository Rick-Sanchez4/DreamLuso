import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'datePt',
  standalone: true
})
export class DatePtPipe implements PipeTransform {
  transform(value: Date | string): string {
    if (!value) return '';
    const date = new Date(value);
    return date.toLocaleDateString('pt-PT', { day: '2-digit', month: '2-digit', year: 'numeric' });
  }
}

