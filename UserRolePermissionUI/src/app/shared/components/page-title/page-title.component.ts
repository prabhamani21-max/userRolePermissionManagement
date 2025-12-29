import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common'; // Add CommonModule
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-page-title',
  standalone: true,
  imports: [CommonModule, RouterModule], // Ensure CommonModule is included
  template: `
    <div class="row">
      <div class="col-12">
        <div class="page-title-box">
          <h4 class="mb-0 fw-semibold">{{ subtitle }}</h4>
          <ol class="breadcrumb mb-0">
            <li class="breadcrumb-item">
              <a [routerLink]="titleLink" [queryParams]="queryParams">{{
                title
              }}</a>
            </li>
            <li class="breadcrumb-item active">{{ subtitle }}</li>
          </ol>
        </div>
      </div>
    </div>
  `,
})
export class PageTitleComponent implements OnInit {
  @Input() title: string = '';
  @Input() subtitle: string = '';
  @Input() titleLink: string = '/';
  @Input() queryParams: { [key: string]: any } = {};

  ngOnInit() {
    // Component initialized
  }
}
