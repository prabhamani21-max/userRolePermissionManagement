import { CUSTOM_ELEMENTS_SCHEMA, Component, OnDestroy, inject } from '@angular/core';
import { SimplebarAngularModule } from 'simplebar-angular';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import {
  NgbCollapse,
  NgbCollapseModule,
  NgbTooltipModule,
} from '@ng-bootstrap/ng-bootstrap';
import { CommonModule } from '@angular/common';
import { findAllParent, findMenuItem } from '../../common/utils';
import { LogoBoxComponent } from '../../shared/components/logo-box/logo-box.component';
import {  MenuItem } from '../../common/menu-meta';
import { basePath } from '../../common/constants/constants';
import { AuthenticationService } from '../../core/services/auth.service';
import { MenuService } from '../../core/services/menu.service';
import { Subscription } from 'rxjs';
@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    SimplebarAngularModule,
    RouterModule,
    NgbCollapseModule,
    CommonModule,
    NgbTooltipModule,
    LogoBoxComponent,
  ],
  templateUrl: './sidebar.component.html',
  styles: ``,
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class SidebarComponent implements OnDestroy {
  menuItems: MenuItem[] = [];
  activeMenuItems: string[] = [];
  authService = inject(AuthenticationService);
  menuService = inject(MenuService);

  router = inject(Router);
  trimmedURL = this.router.url?.replaceAll(
    basePath !== '' ? basePath + '/' : '',
    '/',
  );
  private routerSubscription: Subscription;

  constructor() {
    this.routerSubscription = this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.trimmedURL = this.router.url?.replaceAll(
          basePath !== '' ? basePath + '/' : '/',
          '/',
        );
        this._activateMenu();
        setTimeout(() => {
          this.scrollToActive();
        }, 200);
      }
    });
  }

  ngOnInit(): void {
    if (!this.authService.isAuthenticated()) {
      console.warn('User is not authenticated');
      this.menuItems = [];
      return;
    }
    this.menuService.getSidebarMenu().subscribe({
      next: (menu) => {
        this.menuItems = menu.map(item => ({
          ...item,
          link: item.link && !item.link.startsWith('/') ? '/' + item.link : item.link,
          subMenu: item.subMenu ? item.subMenu.map(sub => ({
            ...sub,
            link: sub.link && !sub.link.startsWith('/') ? '/' + sub.link : sub.link
          })) : []
        }));
        setTimeout(() => {
          this._activateMenu();
          this.scrollToActive();
        }, 200);
      },
      error: (error) => {
        console.error('Failed to load sidebar menu', error);
        this.menuItems = [];
      }
    });
  }

  scrollToActive(): void {
    const activatedItem = document.querySelector('.nav-item li a.active');
    if (activatedItem) {
      const simplebarContent = document.querySelector(
        '.main-nav .simplebar-content-wrapper',
      );
      if (simplebarContent) {
        const activatedItemRect = activatedItem.getBoundingClientRect();
        const simplebarContentRect = simplebarContent.getBoundingClientRect();
        const activatedItemOffsetTop =
          activatedItemRect.top + simplebarContent.scrollTop;
        const centerOffset =
          activatedItemOffsetTop -
          simplebarContentRect.top -
          simplebarContent.clientHeight / 2 +
          activatedItemRect.height / 2;
        this.scrollTo(simplebarContent, centerOffset, 600);
      }
    }
  }

  easeInOutQuad(t: number, b: number, c: number, d: number): number {
    t /= d / 2;
    if (t < 1) return (c / 2) * t * t + b;
    t--;
    return (-c / 2) * (t * (t - 2) - 1) + b;
  }

  scrollTo(element: Element, to: number, duration: number): void {
    const start = element.scrollTop;
    const change = to - start;
    const increment = 20;
    let currentTime = 0;

    const animateScroll = () => {
      currentTime += increment;
      const val = this.easeInOutQuad(currentTime, start, change, duration);
      element.scrollTop = val;
      if (currentTime < duration) {
        setTimeout(animateScroll, increment);
      }
    };
    animateScroll();
  }
_activateMenu(): void {
  const div = document.querySelector('.navbar-nav');

  let matchingMenuItem = null;

  if (div) {
    let items: any = div.getElementsByClassName('nav-link-ref');
    for (let i = 0; i < items.length; ++i) {
      if (
        this.trimmedURL === items[i].pathname ||
        (this.trimmedURL.startsWith('/invoice/') &&
          items[i].pathname === '/invoice/RB6985') ||
        (this.trimmedURL.startsWith('/ecommerce/product/') &&
          items[i].pathname === '/ecommerce/product/1')
      ) {
        matchingMenuItem = items[i];
        break;
      }
    }

    if (matchingMenuItem) {
      const mid = matchingMenuItem.getAttribute('aria-controls');

      const activeMt = findMenuItem(this.menuItems, mid);

      if (activeMt) {
        const matchingObjs = [
          activeMt.key || '',
          ...findAllParent(this.menuItems, activeMt),
        ];

        this.activeMenuItems = matchingObjs;
        this.menuItems.forEach((menu: MenuItem) => {
          menu.collapsed = !matchingObjs.includes(menu.key || '');
        });
      }
    }
  }
}

  hasSubmenu(menu: MenuItem): boolean {
    return !!menu.subMenu && menu.subMenu.length > 0;
}

  toggleMenuItem(menuItem: MenuItem, collapse: NgbCollapse): void {
    collapse.toggle();
    let openMenuItems: string[];
    if (!menuItem.collapsed) {
      openMenuItems = [
        menuItem.key || '',
        ...findAllParent(this.menuItems, menuItem),
      ];
      this.menuItems.forEach((menu: MenuItem) => {
        if (!openMenuItems.includes(menu.key || '')) {
          menu.collapsed = true;
        }
      });
    }
  }

  changeSidebarSize() {
    let size = document.documentElement.getAttribute('data-menu-size');
    if (size == 'sm-hover') {
      size = 'sm-hover-active';
    } else {
      size = 'sm-hover';
    }
    document.documentElement.setAttribute('data-menu-size', size);
  }

  ngOnDestroy(): void {
    this.routerSubscription.unsubscribe();
  }
}