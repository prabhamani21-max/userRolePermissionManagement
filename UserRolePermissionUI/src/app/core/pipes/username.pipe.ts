import { Pipe, PipeTransform } from '@angular/core';
import { UserModel } from '../models/user.model';
import { UserService } from '../services/user.service';
import { Observable, of, map } from 'rxjs';

@Pipe({
  name: 'userName',
  standalone: true
})
export class UserNamePipe implements PipeTransform {
  constructor(private userService: UserService) {}

  transform(userId: number | null | undefined, users: UserModel[]): Observable<string> {
    if (!userId) return of('Unassigned'); // Handles null and undefined

    // Check if we have this user in the provided users array
    const user = users.find((u) => u.id === userId);
    if (user) {
      return of(user.name);
    } else {
      // Fetch the user by ID if not in the provided array
      return this.userService.getUserById(userId).pipe(
        map(response => response ? response.name : 'Unknown')
      );
    }
  }
}
