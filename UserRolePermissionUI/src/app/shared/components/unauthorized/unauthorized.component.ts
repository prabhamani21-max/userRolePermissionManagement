import { LogoBoxComponent } from '../logo-box/logo-box.component';
import { Component } from '@angular/core';

@Component({
  selector: 'unauthorized',
  standalone: true,
  imports: [LogoBoxComponent],
  templateUrl: './unauthorized.component.html',
  styles: ``,
})
export class Unauthorized {}
