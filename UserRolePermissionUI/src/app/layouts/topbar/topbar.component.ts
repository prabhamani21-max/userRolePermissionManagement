import { changetheme } from '../../store/layout/layout-action';
import { CommonModule, DOCUMENT } from '@angular/common';
import {
  CUSTOM_ELEMENTS_SCHEMA,
  Component,
  EventEmitter,
  Inject,
  Output,
  inject,
  type TemplateRef,
} from '@angular/core';
import {
  NgbDropdownModule,
  NgbOffcanvas,
  NgbOffcanvasModule,
  NgbTooltipModule,
} from '@ng-bootstrap/ng-bootstrap';
import { Store } from '@ngrx/store';
import { SimplebarAngularModule } from 'simplebar-angular';
import { getLayoutColor } from '../../store/layout/layout-selector';
import { logout } from '../../store/authentication/authentication.actions';
import { Router, RouterLink } from '@angular/router';
import { AuthenticationService } from '../../core/services/auth.service';
 // import { /*activityStreamData*/ notificationsData } from './data';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [
    NgbDropdownModule,
    SimplebarAngularModule,
    NgbOffcanvasModule,
    CommonModule,
    NgbTooltipModule
  ],
  templateUrl: './topbar.component.html',
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class TopbarComponent {
  element: any;
  profileImageUrl: string = 'assets/images/users/dummy-avatar.jpg';
  currentUser: any;

  router = inject(Router);
  store = inject(Store);
  offcanvasService = inject(NgbOffcanvas);
  // Add these new properties
  userInitials: string = '';
  userFullName: string = '';
  userId:number=0;

  // notificationList = notificationsData;

  // activityList = activityStreamData;
  private authService = inject(AuthenticationService);

  constructor(@Inject(DOCUMENT) private document: any) {}
  @Output() settingsButtonClicked = new EventEmitter();
  @Output() mobileMenuButtonClicked = new EventEmitter();

  ngOnInit(): void {
    this.element = document.documentElement;
    this.loadUserData();
  }
  // Add this new method
  private loadUserData(): void {
    const decodedToken = this.authService.getUserInformation();
    if (decodedToken) {
      this.userFullName = decodedToken.name || 'User';
      // this.userInitials = this.getInitials(this.userFullName);
      this.userId=+decodedToken.userId;
    }
  }
  handleImageError(event: Event): void {
    const imgElement = event.target as HTMLImageElement;
    imgElement.src = 'assets/images/users/dummy-avatar.jpg';
  }

  settingMenu() {
    this.settingsButtonClicked.emit();
  }

  /**
   * Toggle the menu bar when having mobile screen
   */
  toggleMobileMenu() {
    // document.getElementById('topnav-hamburger-icon')?.classList.toggle('open');
    this.mobileMenuButtonClicked.emit();
  }

  // Change Theme
  changeTheme() {
    const color = document.documentElement.getAttribute('data-bs-theme');
    if (color == 'light') {
      this.store.dispatch(changetheme({ color: 'dark' }));
    } else {
      this.store.dispatch(changetheme({ color: 'light' }));
    }
    this.store.select(getLayoutColor).subscribe((color) => {
      document.documentElement.setAttribute('data-bs-theme', color);
    });
  }

  open(content: TemplateRef<any>) {
    this.offcanvasService.open(content, {
      position: 'end',
      panelClass: 'border-0 width-auto',
    });
  }

  logout() {
    this.authService.logout();
  }

  getFileExtensionIcon(file: any) {
    const dotIndex = file.lastIndexOf('.');
    const extension = file.slice(dotIndex + 1);
    if (extension == 'docs') {
      return 'bxs-file-doc';
    } else if (extension == 'zip') {
      return 'bxs-file-archive';
    } else {
      return 'bxl-figma ';
    }
  }
goToProfile() {
  this.router.navigate(['/userRolePermission/dashboard/profile']);
}

}
