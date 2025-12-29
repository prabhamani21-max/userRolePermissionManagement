import { createSelector } from '@ngrx/store';
import { RootReducerState } from '../index';

export const selectAuthenticationState = (state: RootReducerState) => state.authentication;

export const selectIsAuthenticated = createSelector(
  selectAuthenticationState,
  (state) => state.isAuthenticated
);

export const selectUser = createSelector(
  selectAuthenticationState,
  (state) => state.user
);