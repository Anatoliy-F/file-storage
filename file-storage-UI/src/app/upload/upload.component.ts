import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { FormGroup, FormControl, Validators, AbstractControl, AsyncValidatorFn } from "@angular/forms";

import { BaseFormComponent } from "../base-form.component";
import { HttpClient } from "@angular/common/http";
import { environment } from "src/environments/environment";

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.scss']
})

export class UploadComponent extends BaseFormComponent implements OnInit {
    file_store?: FileList;
    file_list: Array<string> = [];

    constructor(
        private http: HttpClient
    ){
        super();
    }

    ngOnInit(): void {
        this.form = new FormGroup({
            note: new FormControl('', Validators.required),
            file: new FormControl('', Validators.required)
        });
    }

    onFileInputChange(list: FileList): void{
        this.file_store = list;
        if(list.length){
            const f = list[0];
            const count = list.length > 1 ? `(+${list.length - 1})` : '';
            this.form.controls['file'].patchValue(`${f.name}${count}`);
        } else {
            this.form.controls['file'].patchValue('');
        }
    }

    onSubmit(){
        const formData = new FormData();
        if(!this.file_store?.length){
            return;
        }
        let fileToUpload = this.file_store[0];
        let note = this.form.controls['note'].value;

        formData.append('file', fileToUpload, fileToUpload.name);
        formData.append('note', note);

        this.http.post(environment.baseUrl + 'upload', formData)
            .subscribe(result => {
                console.log('File is uploaded');
                console.log(result);
            }, error => {
                console.log(error);
            });
    }
}

// interface fileUpload {
//     note: string,
//     file: File
// }

// import { Component, OnInit, Output, EventEmitter } from '@angular/core';
// import { HttpClient, HttpEventType, HttpErrorResponse } from '@angular/common/http';

// import { environment } from 'src/environments/environment';
// import { emitDistinctChangesOnlyDefaultValue } from '@angular/compiler';

// @Component({
//   selector: 'app-upload',
//   templateUrl: './upload.component.html',
//   styleUrls: ['./upload.component.scss']
// })
// export class UploadComponent implements OnInit {
//   progress?: number; //lohanka esta mi loka?
//   //fileSize?: number;
//   message?: string; //lohanka esta mi loka?
//   @Output() public onUploadFinished = new EventEmitter();

//   constructor(private http: HttpClient) { }

//   ngOnInit(): void {
//   }

//   uploadFile(files: FileList){
//     if(files.length === 0){
//       return;
//     }

//     let fileToUpload = <File>files[0];
//     //this.fileSize = fileToUpload.length
//     const formData = new FormData();
//     formData.append('file', fileToUpload, fileToUpload.name);

//     this.http.post(environment.baseUrl + 'upload', formData, {reportProgress: true, observe: 'events'})
//       .subscribe({
//         next: (event) => {
//           if(event.type === HttpEventType.UploadProgress){
//             this.progress = Math.round(100 * event.loaded / event.total!);
//           }
//           if(event.type === HttpEventType.Response){
//             this.message = 'Upload success';
//             this.onUploadFinished.emit(event.body);
//           }
//         },
//         error: (err: HttpErrorResponse) => console.log(err)
//       });
//   }
// }
