import { Component } from '@angular/core'
import { CommonModule } from '@angular/common'
import { RouterOutlet } from '@angular/router'
import { AuthenticationService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  title = 'userRolePermission';
  constructor(public authService: AuthenticationService) {}

  // Optional getter to simplify template usage
  get isLoggedIn(): boolean {
    return this.authService.isAuthenticated();
  }

}