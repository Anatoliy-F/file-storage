import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ReactiveFormsModule } from '@angular/forms';

import { AngularMaterialModule } from './angular-material.module';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { FetchFilesComponent } from './fetch-files/fetch-files.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { UploadComponent } from './upload/upload.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { LoginComponent } from './auth/login/login.component';
import { AuthInterceptor } from './auth/auth.interceptor';
import { RegistrationComponent } from './auth/registration/registration.component';
import { FileDataComponent } from './fetch-files/file-data/file-data.component';
import { FileDataEditComponent } from './fetch-files/file-data-edit/file-data-edit.component';
import { ShareFileComponent } from './fetch-files/share-file/share-file.component';
import { ManageUsersComponent } from './manage-users/manage-users.component';
import { UserEditComponent } from './manage-users/user-edit/user-edit.component';
import { FetchSharedFilesComponent } from './fetch-files/fetch-shared-files/fetch-shared-files.component';
import { SharedFileDataComponent } from './fetch-files/shared-file-data/shared-file-data.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    FetchFilesComponent,
    NavMenuComponent,
    UploadComponent,
    LoginComponent,
    RegistrationComponent,
    FileDataComponent,
    FileDataEditComponent,
    ShareFileComponent,
    ManageUsersComponent,
    UserEditComponent,
    FetchSharedFilesComponent,
    SharedFileDataComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    AngularMaterialModule,
    ReactiveFormsModule
  ],
  providers: [{
    provide: HTTP_INTERCEPTORS,
    useClass: AuthInterceptor,
    multi: true
  }],
  bootstrap: [AppComponent]
})
export class AppModule { }
