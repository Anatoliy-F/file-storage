import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router'; 
import { FormGroup, FormControl, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';

import { BaseFormComponent } from 'src/app/base-form.component';
import { AuthService } from '../auth.service';
import { RegistrationRequest } from '../registration-request';
import { RegistrationResult } from '../registration-result';


@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss']
})
export class RegistrationComponent extends BaseFormComponent implements OnInit {
  title?: string;
  registrationResult?: RegistrationResult;

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) {
    super()
   }

  ngOnInit(): void {
    this.form = new FormGroup({
      userName: new FormControl('', [
        Validators.required,
        Validators.pattern(/^[a-zA-Z0-9]{3,30}$/)
      ]),
      email: new FormControl('', [
        Validators.required,
        Validators.email
      ]),
      password: new FormControl('', [
        Validators.required
      ]),
      //TODO: validate confirm password
      confirm: new FormControl('', [
        Validators.required
      ])
    });
  }

  onSubmit(){
    let registrationRequest = <RegistrationRequest>{};
    registrationRequest.userName = this.form.controls['userName'].value;
    registrationRequest.email = this.form.controls['email'].value;
    registrationRequest.password = this.form.controls['password'].value;
    registrationRequest.confirmPassword = this.form.controls['confirm'].value;

    this.authService.registration(registrationRequest)
      .subscribe(result => {
        console.log(result);
        this.registrationResult = result;
        if(result.success){
          //TODO: change registration logic
          this.router.navigate(['login']);
        }
      }, error => {
        console.log(error);
      });
  }

}
