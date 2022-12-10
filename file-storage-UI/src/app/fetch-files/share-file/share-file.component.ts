import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { Observable, Subject } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';
import { Location } from '@angular/common';

import { FileData } from '../fileData';
import { User } from '../fileData';
import { BaseFormComponent } from 'src/app/base-form.component';
import { FetchFilesService } from '../fetch-files.service';

@Component({
  selector: 'app-share-file',
  templateUrl: './share-file.component.html',
  styleUrls: ['./share-file.component.scss']
})
export class ShareFileComponent extends BaseFormComponent implements OnInit {

  title?: string;
  fileData?: FileData;
  id?: string;

  private destroySubject = new Subject();



  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private fetchFileService: FetchFilesService,
    private location: Location
  ) { 
    super();
  }

  ngOnInit(): void {
    console.log("On init start");
    this.form = new FormGroup({
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
      this.fetchFileService.get(this.id)
        .subscribe(result => {
          this.fileData = result;
        }, error => {
          console.error(error);
        });
    }
  }

  onBack(){
    this.location.back();
  }

  onSubmit(){
    if(this.fileData && !this.fileData.viewers.find(i => i == this.form.controls['email'].value)){
      
      let user: User = {
        email: this.form.controls['email'].value,
        concurrency: '',
        name: '',
        id: ''
      }
      //this.fileData.viewers.push(user);

      console.log('lets do it');
      console.log(JSON.stringify(this.fileData));

      this.fetchFileService.share(this.fileData, this.form.controls['email'].value)
        .subscribe(result => {
          console.log(result);
          this.fileData = result;
          
        }, error => {
          console.log(error);
        })
    }
  }

}
