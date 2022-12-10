import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subject, tap } from 'rxjs';

import { environment } from 'src/environments/environment'; 
import { LoginRequest } from './login-request';
import { LoginResult } from './login-result';
import { RegistrationRequest } from './registration-request';
import { RegistrationResult } from './registration-result';

@Injectable({
    providedIn: 'root',
})

export class AuthService {
    constructor(
        protected http: HttpClient
    ){}

    private tokenKey: string = "token";
    private adminKey: string = "isAdmin";

    private _authStatus = new Subject<boolean>();
    public authStatus = this._authStatus.asObservable();

    isAuthenticated() : boolean {
        return this.getToken() !== null;
    }

    isAdmin() : boolean {
        return (localStorage.getItem(this.adminKey) === "true");
    }

    getToken(): string | null {
        return localStorage.getItem(this.tokenKey);
    }

    login(item: LoginRequest): Observable<LoginResult> {
        let url = environment.baseUrl + 'Account/Login';
        return this.http.post<LoginResult>(url, item)
            .pipe(tap(loginResult => {
                if(loginResult.success && loginResult.token){
                    localStorage.setItem(this.tokenKey, loginResult.token);
                    this.setAuthStatus(true);
                }
                if(loginResult.isAdmin){
                    localStorage.setItem(this.adminKey, "true");
                }
            }));
    }

    logout() {
        localStorage.removeItem(this.tokenKey);
        localStorage.removeItem(this.adminKey);
        this.setAuthStatus(false);
    }

    init() : void {
        if(this.isAuthenticated()){
            this.setAuthStatus(true);
        }
    }

    //TODO: check registration
    registration(item: RegistrationRequest): Observable<RegistrationResult> {
        let url = environment.baseUrl + 'Account/Registration';
        return this.http.post<RegistrationResult>(url, item);
    }

    private setAuthStatus(isAuthenticated: boolean): void{
        this._authStatus.next(isAuthenticated);
    }
}