import { Directive, Input, TemplateRef, ViewContainerRef, OnInit } from '@angular/core';
import { AuthenticationService } from '../../core/services/auth.service';
import { ScreenService } from '../../core/services/screen.service';
import { ScreenActionDto } from '../../core/models/screen-action.model';

@Directive({
  selector: '[hasPermission]',
  standalone: true
})
export class HasPermissionDirective implements OnInit {
  @Input('hasPermission') permissionKey!: string;
  private actions: ScreenActionDto[] = [];

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private authService: AuthenticationService,
    private screenService: ScreenService
  ) {}

  ngOnInit() {
    this.screenService.getAllScreenActions().subscribe({
      next: (actions) => {
        this.actions = actions;
        this.checkPermission();
      },
      error: () => {
        this.viewContainer.clear(); // Hide if error
      }
    });
  }

  private checkPermission() {
    const action = this.actions.find(a => a.key === this.permissionKey);
    if (!action) {
      this.viewContainer.clear();
      return;
    }
    const userPermissions = this.authService.getUserPermissions();
    if (userPermissions.includes(action.id.toString())) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainer.clear();
    }
  }
}