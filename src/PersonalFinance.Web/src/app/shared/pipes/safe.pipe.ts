import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

@Pipe({
  name: 'safe',
  standalone: true
})
export class SafePipe implements PipeTransform {
  constructor(private sanitizer: DomSanitizer) {}

  transform(value: string, type: string): SafeHtml | string {
    if (type === 'html') {
      // Use sanitize instead of bypass to maintain XSS protection
      const sanitized = this.sanitizer.sanitize(1, value); // SecurityContext.HTML = 1
      return sanitized || '';
    }
    return value;
  }
}
