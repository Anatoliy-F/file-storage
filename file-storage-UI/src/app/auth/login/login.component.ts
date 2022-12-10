import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router'; 
import { FormGroup, FormControl, Validators } from '@angular/forms';

import { BaseFormComponent } from 'src/app/base-form.component';
import { AuthService } from '../auth.service';
import { LoginRequest } from '../login-request';
import { LoginResult } from '../login-result';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent extends BaseFormComponent implements OnInit {

  title?: string;
  loginResult? : LoginResult;

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) { 
    super()
  }

  ngOnInit(): void {
    this.form = new FormGroup({
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', Validators.required)
    });
  }

  onSubmit(){
    let LoginRequest = <LoginRequest>{};
    LoginRequest.email = this.form.controls['email'].value;
    LoginRequest.password = this.form.controls['password'].value;

    this.authService
      .login(LoginRequest)
      .subscribe(result => {
        console.log(result);
        this.loginResult = result;
        if(result.success) {
          this.router.navigate(["/"]);
        }
      }, error => {
        console.log(error);
        if(error.status == 401){
          this.loginResult = error.error;
        }
      })
  }

}
