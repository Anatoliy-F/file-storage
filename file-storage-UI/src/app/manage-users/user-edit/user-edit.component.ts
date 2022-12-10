import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { Observable, Subject } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';
import { Location } from '@angular/common';

import { AppUser } from '../app-user';
import { BaseFormComponent } from 'src/app/base-form.component';
import { AppUserService } from '../app-user.service';

@Component({
  selector: 'app-user-edit',
  templateUrl: './user-edit.component.html',
  styleUrls: ['./user-edit.component.scss']
})
export class UserEditComponent extends BaseFormComponent implements OnInit {
  title?: string;
  user?: AppUser;
  id?: string;

  private destroySubject = new Subject();

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private appUserServie: AppUserService,
    private location: Location
  ) { 
    super();
  }

  ngOnInit(): void {
    console.log("On init start");
    this.form = new FormGroup({
      name: new FormControl('', [
        Validators.required,
        Validators.pattern(/^[a-zA-Z0-9]{3,30}$/)
      ]),
      email: new FormControl('', [
        Validators.required,
        Validators.email
      ]),
    });

    this.loadData();
  }

  loadData(){
    this.id = this.activatedRoute.snapshot.paramMap.get('id')!;

    if(this.id){
      this.appUserServie.get(this.id)
        .subscribe(result => {
          this.user = result;
          this.form.patchValue(this.user);
        }, error => {
          console.error(error);
        });
    }
  }

  onBack(){
    this.location.back();
  }

  onDelete(){
    if(this.user){
      this.appUserServie.delete(this.user)
      .subscribe(result => {
        console.log("deleted");
        this.location.back();
      }, error => {
        console.log(error);
      });
    }  
  }

  onSubmit(){
    if(this.user){
      this.user.name = this.form.controls['name'].value;
      this.user.email = this.form.controls['email'].value;

      console.log(this.user);

      this.appUserServie.put(this.user)
        .subscribe(result => {
          console.log(result);
        }, error => {
          console.log(error);
        })
    }
  }
}
