import { Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';

@Pipe({ name: 'shorten' })
export class StringShortenPipe implements PipeTransform {

  constructor(private datePipe: DatePipe) { }

  transform(value: string, option: string): string {
    switch (option) {
      case 'first':
        return value.split(' ')[0];
      case 'percent':
        value = value.toString();
        if (value.includes('.')) {
          var parts = value.split('.');
          return parts[0] +  '.' + parts[1].substring(0, (parts[1].length > 1 ? 1 : parts[1].length)) + '%';
        }
        else
          return value + '%';
      case 'date':
        return this.datePipe.transform(value, 'MMM/dd/yyyy');
      default:
        return value;
    }
  }
}
