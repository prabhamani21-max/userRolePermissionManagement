import { createReducer, on } from '@ngrx/store';
import { logout } from './authentication.actions';

export interface AuthenticationState {
  isAuthenticated: boolean;
  user: any;
}

export const initialState: AuthenticationState = {
  isAuthenticated: false,
  user: null,
};

export const authenticationReducer = createReducer(
  initialState,
  on(logout, (state) => ({
    ...state,
    isAuthenticated: false,
    user: null,
  }))
);