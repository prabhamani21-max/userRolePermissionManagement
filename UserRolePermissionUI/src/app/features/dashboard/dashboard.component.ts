import { Component, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { RouterModule } from '@angular/router';
import { PageTitleComponent } from '../../shared/components/page-title/page-title.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterModule, PageTitleComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class DashboardComponent {
  dashboardCards = [
    {
      icon: 'mdi:account', // User dashboard
      iconColor: 'primary',
      title: 'User Dashboard',
      path: '/admin/users',
      badgeColor: 'primary',
      badgeIcon: 'bx bx-user',
    },
    {
      icon: 'mdi:briefcase', // Client dashboard
      iconColor: 'success',
      title: 'Client Dashboard',
      path: '/userRolePermission/dashboard/clientlist',
      badgeColor: 'success',
      badgeIcon: 'bx bx-briefcase',
    },
    {
      icon: 'mdi:chart-areaspline',
      iconColor: 'info',
      title: 'Sales Dashboard',
      path: '/userRolePermission/dashboard/sales',
      badgeColor: 'info',
      badgeIcon: 'bx bx-trending-up',
    },

    {
      icon: 'mdi:chart-areaspline', // Lead Management
      iconColor: 'info',
      title: 'BDE Dashboard',
      path: '/userRolePermission/dashboard/bde',
      badgeColor: 'info',
      badgeIcon: 'bx bx-trending-up',
    },
    {
      icon: 'mdi:chart-areaspline', // Reusing lead icon
      iconColor: 'info',
      title: 'BDM Dashboard',
      path: '/userRolePermission/dashboard/bdm',
      badgeColor: 'info',
      badgeIcon: 'bx bx-trending-up',
    },
    //  {
    //   icon: 'mdi:chart-areaspline',
    //   iconColor: 'info',
    //   title: 'Leads Dashboard',
    //   path: '/userRolePermission/dashboard/leads',
    //   badgeColor: 'info',
    //   badgeIcon: 'bx bx-trending-up',
    // },

    {
      icon: 'mdi:calendar-clock', // HR dashboard
      iconColor: 'warning',
      title: 'HR Dashboard',
      path: '/userRolePermission/HR',
      badgeColor: 'warning',
      badgeIcon: 'bx bx-calendar',
    },
    {
      icon: 'mdi:account-details', // Applicant management
      iconColor: 'danger',
      title: 'Applicant Dashboard',
      path: '/userRolePermission/dashboard/applicant/applicants',
      badgeColor: 'danger',
      badgeIcon: 'bx bx-file',
    },
  ];
}
