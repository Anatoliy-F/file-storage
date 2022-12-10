import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BaseService, ApiResult } from '../base.service';
import { Observable } from 'rxjs';

import { AppUser } from './app-user';

@Injectable({
    providedIn: 'root',
  })

  export class AppUserService extends BaseService<AppUser>{
    
    constructor(
        http: HttpClient
    ) {
        super(http);
    }

    getData(
        pageIndex: number,
        pageSize: number,
        sortColumn: string,
        sortOrder: string,
        filterColumn: string | null,
        filterQuery: string | null
      ): Observable<ApiResult<AppUser>> {
        let url = this.getUrl("account");

        let params = new HttpParams()
        .set("pageIndex", pageIndex.toString())
        .set("pageSize", pageSize.toString())
        .set("sortColumn", sortColumn)
        .set("sortOrder", sortOrder);
  
        if (filterColumn && filterQuery) {
            params = params
            .set("filterColumn", filterColumn)
            .set("filterQuery", filterQuery);
        }

        return this.http.get<ApiResult<AppUser>>(url, { params })
      }

      get(id: string): Observable<AppUser>
      {
        let url = this.getUrl("account/" + id);
        return this.http.get<AppUser>(url);
      }

      getByEmail(email: string): Observable<AppUser>
      {
        let url = this.getUrl("account/byEmail/" + email);
        return this.http.get<AppUser>(url);
      }

      put(item: AppUser): Observable<AppUser>{
        let url = this.getUrl("account/" + item.id);
        return this.http.put<AppUser>(url, item);
      }

      delete(item: AppUser): Observable<AppUser>{
        let url = this.getUrl("account/" + item.id);
        return this.http.delete<AppUser>(url, { body: item });
      }
  }

