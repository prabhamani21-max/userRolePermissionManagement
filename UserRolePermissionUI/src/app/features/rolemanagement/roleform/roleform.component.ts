import {
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  OnInit,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, Subject } from 'rxjs';
import { take, tap, finalize, takeUntil } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { CanComponentDeactivate } from '../../../core/guards/unsaved-changes.guard';
import { Role } from '../../../core/models/role.model';
import { RoleService } from '../../../core/services/role.service';
import { ConfirmationService } from '../../../shared/components/confirm-dialog/confirm-dialog.service';
import { PageTitleComponent } from '../../../shared/components/page-title/page-title.component';

@Component({
  selector: 'app-roleform',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, PageTitleComponent],
  templateUrl: './roleform.component.html',
  styleUrl: './roleform.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RoleFormComponent implements OnInit, CanComponentDeactivate {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly roleService = inject(RoleService);
  private readonly toastr = inject(ToastrService);
  private readonly confirmationService = inject(ConfirmationService);
  private readonly cdRef = inject(ChangeDetectorRef);

  roleForm!: FormGroup;
  isEditMode = false;
  isViewing = false;
  isSubmitting = false;
  isLoading = false;
  errorMessage: string = '';
  successMessage: string = '';
  currentId: number | null = null;

  private destroy$ = new Subject<void>();

  ngOnInit(): void {
    this.route.paramMap.pipe(take(1)).subscribe((params) => {
      const id = params.get('id');
      const routePath = this.route.snapshot.routeConfig?.path;
      this.isViewing = routePath === 'view/:id';
      this.isEditMode = routePath === 'edit/:id';
      this.initializeForm();
      if (this.isViewing) {
        this.roleForm.disable();
      }
      if (this.isEditMode || this.isViewing) {
        this.currentId = +id!;
        this.loadRoleData(this.currentId);
      }
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private initializeForm(): void {
    this.roleForm = this.fb.group({
      ...(this.isEditMode && { id: [null] }),
      name: [
        { value: '', disabled: this.isViewing },
        [Validators.required, Validators.minLength(2)],
      ],
      status: [{ value: null, disabled: this.isViewing }, Validators.required],
    });
  }

  private loadRoleData(id: number): void {
    this.isLoading = true;
    this.roleService
      .getRoleById(id)
      .pipe(
        tap((response) => this.handleRoleResponse(response)),
        finalize(() => {
          this.isLoading = false;
          this.cdRef.markForCheck();
        }),
      )
      .subscribe();
  }

  private handleRoleResponse(response: Role): void {
    if (response) {
      const role = response;
      this.roleForm.patchValue({
        id: role.id,
        name: role.name,
        status: role.status,
      });
      if (this.isViewing) {
        this.roleForm.disable();
      }
    } else {
      this.toastr.warning('Role data not found', 'Warning');
      this.navigateBack();
    }
  }

  onSubmit(): void {
    if (this.roleForm.invalid || this.isSubmitting) {
      this.markFormAsTouched();
      return;
    }

    this.isSubmitting = true;
    const roleData = this.prepareRoleData();

    let submit$: Observable<Role>;

    submit$ = this.roleService.addEditRole(roleData);

    submit$
      .pipe(
        takeUntil(this.destroy$),
        tap((response) => this.handleSubmitResponse(response)),
        finalize(() => {
          this.isSubmitting = false;
          this.cdRef.markForCheck();
        }),
      )
      .subscribe();
  }

  private prepareRoleData(): Role {
    const formValue = this.roleForm.getRawValue();
    const role: Role = {
      id: this.isEditMode ? formValue.id : 0,
      name: formValue.name.trim(),
      status: formValue.status,
    };
    return role;
  }

  private handleSubmitResponse(response: Role): void {
    if (response) {
      const action = this.isEditMode ? 'updated' : 'created';
      this.toastr.success(`Role ${action} successfully`, 'Success');
      this.roleForm.markAsPristine();
      this.navigateBack(response.id);
    } else {
      this.toastr.error('Failed to save role', 'Error');
    }
  }

  canDeactivate(): Observable<boolean> | Promise<boolean> | boolean {
    if (this.roleForm.dirty) {
      return this.confirmationService.confirm(
        'Unsaved Changes',
        'You have unsaved changes. Do you really want to leave?',
        'Yes',
        'Cancel',
      );
    }
    return true;
  }

  onCancel(): void {
    this.navigateBack();
  }

  private navigateBack(updatedId?: number): void {
    this.router.navigate([`userRolePermission/rolemanagement`]);
  }

  private markFormAsTouched(): void {
    Object.values(this.roleForm.controls).forEach((control) => {
      control.markAsTouched();
    });
  }

  isInvalid(controlName: string): boolean {
    const control = this.roleForm.get(controlName);
    return !!control && control.invalid && control.touched;
  }

  get submitButtonText(): string {
    return this.isEditMode ? 'Update' : 'Create';
  }
}
