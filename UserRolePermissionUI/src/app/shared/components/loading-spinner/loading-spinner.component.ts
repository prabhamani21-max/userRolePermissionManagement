import { Component } from '@angular/core';
import { LoadingService } from '../../../core/services/loading.service';
import { CommonModule } from '@angular/common';
import { NgIf, AsyncPipe } from '@angular/common';

@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  imports: [CommonModule, NgIf, AsyncPipe],
  template: `
    <div *ngIf="loadingService.loading$ | async"
     class="position-absolute top-50 start-50 translate-middle"
     style="z-index: 999;">
  <div class="spinner-border text-primary" role="status">
    <span class="visually-hidden">Loading...</span>
  </div>
</div>
  `,
})
export class LoadingSpinnerComponent {
  constructor(public loadingService: LoadingService) {}
}