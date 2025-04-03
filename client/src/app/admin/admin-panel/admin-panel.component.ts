import { Component } from '@angular/core';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { UserManagmentComponent } from "../user-managment/user-managment.component";
import { HasRoleDirective } from '../../_directives/has-role.directive';
import { PhotoManagementComponent } from "../photo-managment/photo-managment.component";

@Component({
  selector: 'app-admin-panel',
  imports: [TabsModule, UserManagmentComponent, HasRoleDirective, PhotoManagementComponent],
  templateUrl: './admin-panel.component.html',
  styleUrl: './admin-panel.component.css'
})
export class AdminPanelComponent {

}
