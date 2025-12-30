import { Component, CUSTOM_ELEMENTS_SCHEMA, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { PageTitleComponent } from '../../shared/components/page-title/page-title.component';
 // Assumed service for applicants
import { ToastrService } from 'ngx-toastr';
import { Pagination } from '../../core/models/pagination.model';
import { AuthenticationService } from '../../core/services/auth.service';
import { RoleEnum } from '../../core/enums/role.enum';
//import { Applicant } from '../../core/models/applicant.model'; // Assumed model for applicants

@Component({
  selector: 'app-hrdashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, PageTitleComponent],
  templateUrl: './hrdashboard.component.html',
  styleUrls: ['./hrdashboard.component.scss'],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class HRDashboardComponent implements OnInit {
  constructor(private router: Router) {}

  titleLink: string = '/userRolePermission/HR';

  dashboardCards: any[] = [
    {
      icon: 'mdi:briefcase-check',
      iconColor: 'primary',
      title: 'Jobs Assigned',
      count: 0,
      path: '/salexiHR/dashboard/job',
      badgeColor: 'primary',
      badgeIcon: 'bx bx-briefcase',
    },
    {
      icon: 'mdi:clock-alert',
      iconColor: 'warning',
      title: 'Jobs Approaching Deadline',
      count: 0,
      path: '/salexiHR/dashboard/job',
      badgeColor: 'warning',
      badgeIcon: 'bx bx-time',
    },
    {
      icon: 'mdi:new-box',
      iconColor: 'info',
      title: 'Job Exceded Deadline',
      count: 0,
      path: '/salexiHR/dashboard/job',
      badgeColor: 'info',
      badgeIcon: 'bx bx-star',
    },
    {
      icon: 'mdi:account-group',
      iconColor: 'info',
      title: 'Applicant List',
      count: 0,
      path: '/salexiHR/dashboard/applicant/applicants',
      badgeColor: 'info',
      badgeIcon: 'bx bx-user',
    },
  ];

  ngOnInit(): void {
    // Initialization logic here
  }

  onCardClick(card: any): void {
    this.router.navigate([card.path]);
  }
}