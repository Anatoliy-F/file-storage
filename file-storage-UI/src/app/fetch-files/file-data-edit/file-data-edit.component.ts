import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { Observable, Subject } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';
import { Location } from '@angular/common';

import { FileData } from '../fileData';
import { BaseFormComponent } from 'src/app/base-form.component';
import { FetchFilesService } from '../fetch-files.service';

@Component({
  selector: 'app-file-data-edit',
  templateUrl: './file-data-edit.component.html',
  styleUrls: ['./file-data-edit.component.scss']
})
export class FileDataEditComponent extends BaseFormComponent implements OnInit {
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
      name: new FormControl('', [
        Validators.required,
        Validators.pattern(/^[a-zA-Z0-9.#$]{1,100}$/)
      ]),
      note: new FormControl('', Validators.required),
      isPublic: new FormControl('', Validators.required)
    });

    this.loadData();
  }

  loadData(){
    this.id = this.activatedRoute.snapshot.paramMap.get('id')!;

    if(this.id){
      this.fetchFileService.get(this.id)
        .subscribe(result => {
          this.fileData = result;
          this.form.patchValue(this.fileData);
        }, error => {
          console.error(error);
        });
    }
  }

  onBack(){
    this.location.back();
  }

  onSubmit(){
    if(this.fileData){
      this.fileData.name = this.form.controls['name'].value;
      this.fileData.note = this.form.controls['note'].value;
      this.fileData.isPublic = this.form.controls['isPublic'].value;

      console.log(this.fileData);

      this.fetchFileService.put(this.fileData)
        .subscribe(result => {
          console.log(result);
        }, error => {
          console.log(error);
        })
    }
  }

}
