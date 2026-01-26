import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer, SafeHtml, SecurityContext } from '@angular/platform-browser';

@Pipe({
  name: 'safe',
  standalone: true
})
export class SafePipe implements PipeTransform {
  constructor(private sanitizer: DomSanitizer) {}

  transform(value: string, type: string): SafeHtml | string {
    if (type === 'html') {
      // Use sanitize with SecurityContext.HTML to maintain XSS protection
      const sanitized = this.sanitizer.sanitize(SecurityContext.HTML, value);
      return sanitized || '';
    }
    return value;
  }
}
