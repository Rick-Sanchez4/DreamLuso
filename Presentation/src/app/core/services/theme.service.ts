import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  isDarkMode = signal<boolean>(true);

  constructor() {
    // Check for saved theme preference or default to DARK mode
    const savedTheme = localStorage.getItem('theme');
    
    // Default to dark mode unless user explicitly chose light
    if (savedTheme === 'light') {
      this.enableLightMode();
    } else {
      this.enableDarkMode();
    }
  }

  toggleTheme(): void {
    if (this.isDarkMode()) {
      this.enableLightMode();
    } else {
      this.enableDarkMode();
    }
  }

  enableDarkMode(): void {
    this.isDarkMode.set(true);
    document.documentElement.classList.add('dark');
    localStorage.setItem('theme', 'dark');
  }

  enableLightMode(): void {
    this.isDarkMode.set(false);
    document.documentElement.classList.remove('dark');
    localStorage.setItem('theme', 'light');
  }
}

