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
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, of, Subject } from 'rxjs';
import { take, tap, finalize, map, takeUntil } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { CanComponentDeactivate } from '../../../core/guards/unsaved-changes.guard';
import { CreateUser, UserModel } from '../../../core/models/user.model';
import { UserService } from '../../../core/services/user.service';
import { ConfirmationService } from '../../../shared/components/confirm-dialog/confirm-dialog.service';
import { PageTitleComponent } from '../../../shared/components/page-title/page-title.component';
import { Role } from '../../../core/models/role.model';
import { CustomValidators } from '../../../shared/Validators/custom.validators';
import { AuthenticationService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-userform',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    PageTitleComponent,
  ],
  templateUrl: './userform.component.html',
  styleUrl: './userform.component.scss',
})
export class UserFormComponent implements OnInit, CanComponentDeactivate {
  subtitle: string = '';
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly userService = inject(UserService);
  private readonly toastr = inject(ToastrService);
  private readonly confirmationService = inject(ConfirmationService);
  private readonly authService = inject(AuthenticationService);
  private readonly cdRef = inject(ChangeDetectorRef); // ✅ for manual change detection
  userForm!: FormGroup;
  isEditMode = false;
  roles$: Observable<Role[]> = this.authService
    .fetchRoles()
    .pipe(map((response: any) => response.Data ?? response ?? []));
  isLoading = false;
  isViewing = false;
  isSubmitting = false;
  errorMessage: string = '';
  successMessage: string = '';
  checkingEmail: boolean = false;
  checkingContact: boolean = false;
  currentId: number = 0;
  originalEmail: string = '';
  originalContactNo: string = '';

  private destroy$ = new Subject<void>();

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  ngOnInit(): void {
    this.route.paramMap.pipe(take(1)).subscribe((params) => {
      const id = params.get('id');
      // Determine mode based on route path
      const routePath = this.route.snapshot.routeConfig?.path;
      this.isViewing = routePath === 'view/:id';
      this.isEditMode = routePath === 'edit/:id';
      this.initializeForm();
      if (this.isViewing) {
        this.userForm.disable();
      }
      if (this.isEditMode || this.isViewing) {
        this.currentId = +id!;
        this.loadUserData(this.currentId);
        this.subtitle = this.isViewing ? 'View User' : 'Edit User';
      } else {
        this.subtitle = 'Add User';
      }
    });
  }

  private initializeForm(): void {
    this.userForm = this.fb.group(
      {
        ...(this.isEditMode && { id: [null] }),
        name: [{ value: '', disabled: this.isViewing }, Validators.required],
        email: [
          { value: '', disabled: this.isViewing },
          [Validators.required, Validators.email],
          [CustomValidators.emailAvailabilityValidator(this.authService)],
        ],
        contactNo: [
          { value: '', disabled: this.isViewing },
          [Validators.required, Validators.pattern(/^[0-9]{10,15}$/)],
          [CustomValidators.contactAvailabilityValidator(this.authService)],
        ],
        dob: [{ value: '', disabled: this.isViewing }, Validators.required],
        gender: [{ value: '', disabled: this.isViewing }, Validators.required],
        roleId: [{ value: '', disabled: this.isViewing }, Validators.required],
        ...(this.isEditMode && {
          statusId: [
            { value: null, disabled: this.isViewing },
            Validators.required,
          ],
        }),
        ...(!this.isEditMode && {
          password: [
            { value: '', disabled: this.isViewing },
            [CustomValidators.passwordValidator()],
          ],
          confirmPassword: [{ value: '', disabled: this.isViewing }],
        }),
      },
      {
        // If in edit mode, password fields should not be validated
        validators: this.isEditMode
          ? null
          : CustomValidators.passwordMatchValidator,
      },
    );
  }

  private loadUserData(id: number): void {
    this.isLoading = true;
    this.userService
      .getUserById(id)
      .pipe(
        tap((response) => this.handleUserResponse(response)),
        finalize(() => (this.isLoading = false)),
      )
      .subscribe();
  }

  private handleUserResponse(
    response: UserModel,
  ): void {
    if (response) {
      const user = response;
      this.originalEmail = user.email;
      this.originalContactNo = user.contactNo;

      const patchData: any = {
        name: user.name,
        email: user.email,
        contactNo: user.contactNo,
        dob: user.dob,
        gender: user.gender,
        roleId: user.roleId,
      };

      // Type guard to handle UserModel-only fields
      if ('id' in user) {
        patchData.id = user.id;
      }

      this.userForm.patchValue(patchData);

      if (this.isViewing) {
        this.userForm.disable(); // ✅ make all fields readonly
      } else {
        // Set async validators only if editing
        this.userForm
          .get('email')
          ?.setAsyncValidators(
            CustomValidators.emailAvailabilityValidator(
              this.authService,
              this.currentId,
              this.originalEmail,
            ),
          );
        this.userForm
          .get('contactNo')
          ?.setAsyncValidators(
            CustomValidators.contactAvailabilityValidator(
              this.authService,
              this.currentId,
              this.originalContactNo,
            ),
          );

        this.userForm.get('email')?.updateValueAndValidity();
        this.userForm.get('contactNo')?.updateValueAndValidity();
      }
    } else {
      this.toastr.warning('User data not found', 'Warning');
      this.navigateBack();
    }
  }
  onSubmit(): void {
    if (this.userForm.invalid || this.isSubmitting) {
      this.markFormAsTouched();
      return;
    }

    this.isSubmitting = true;
    const userData = this.prepareUserData();
    const formRoleId = +this.userForm.get('roleId')?.value;

    let submit$: Observable<any>;

    if (this.isEditMode) {
      const userModel = userData as UserModel;
      submit$ = this.userService.updateUser(userModel.id, userModel);
    } else {
      const user = userData as CreateUser;

      if (!user.password) {
        this.toastr.error('Password is required for new user registration');
        this.isSubmitting = false;
        return;
      }

      submit$ = this.authService.register(user);
    }

    submit$
      .pipe(
        takeUntil(this.destroy$),
        tap((response) =>
          this.handleSubmitResponse(
            response,
            this.isEditMode ? undefined : formRoleId,
          ),
        ),
        finalize(() => (this.isSubmitting = false)),
      )
      .subscribe();
  }

  private prepareUserData(): CreateUser | UserModel {
    const formValue = this.userForm.getRawValue(); // ✅ includes disabled fields
    if (this.isEditMode) {
      const user: UserModel = {
        id: formValue.id,
        name: formValue.name.trim(),
        email: formValue.email,
        contactNo: formValue.contactNo,
        password: '',
        dob: formValue.dob,
        gender: formValue.gender,
        roleId: formValue.roleId,
        statusId: 1, // Default active status
      };
      return user;
    } else {
      const newUser: CreateUser = {
        name: formValue.name.trim(),
        email: formValue.email,
        password: formValue.password,
        contactNo: formValue.contactNo,
        dob: formValue.dob,
        gender: formValue.gender,
        roleId: formValue.roleId,
        profileImage: formValue.profileImage || '',
      };
      return newUser;
    }
  }
  private handleSubmitResponse(
    response: any,
    formRoleId?: number,
  ): void {
    if (this.isEditMode ? response.message : response.Status) {
      const action = this.isEditMode ? 'updated' : 'created';
      this.toastr.success(`User ${action} successfully`, 'Success');
      this.userForm.markAsPristine();

      if (this.isEditMode) {
        this.navigateBack(); // No id returned for update
      } else {
        // For newly created user, check if roleId is 6 (Client)
        if (formRoleId === 6) {
          this.navigateToClientForm(response.Data);
        } else {
          this.navigateBack(); // For newly created user, we don't have ID
        }
      }
    } else {
      this.toastr.error((this.isEditMode ? response.message : response.Message) || 'Failed to save user', 'Error');
    }
  }

  canDeactivate(): Observable<boolean> | Promise<boolean> | boolean {
    if (this.userForm.dirty) {
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
    this.router.navigate([`userRolePermission/dashboard/user`]);
  }

  private navigateToClientForm(userData: any): void {
    this.router.navigate(['/userRolePermission/dashboard/clientlist/create'], {
      queryParams: {
        userId: userData.id,
        name: userData.name,
        email: userData.email,
        contactNo: userData.contactNo,
        parentRoute: '/userRolePermission/dashboard/user',
      },
    });
  }

  private markFormAsTouched(): void {
    Object.values(this.userForm.controls).forEach((control) => {
      control.markAsTouched();
    });
  }

  isInvalid(controlName: string): boolean {
    const control = this.userForm.get(controlName);
    return !!control && control.invalid && control.touched;
  }

  get submitButtonText(): string {
    return this.isEditMode ? 'Update' : 'Create';
  }

}
