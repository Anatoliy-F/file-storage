import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BaseService, ApiResult } from '../base.service';
import { Observable } from 'rxjs';

import { FileData } from './fileData';
import { ShortLink } from './fileData';

@Injectable({
    providedIn: 'root',
  })

  export class FetchFilesService extends BaseService<FileData>{

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
      ): Observable<ApiResult<FileData>> {
        let url = this.getUrl("file");

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

        return this.http.get<ApiResult<FileData>>(url, { params })
      }
      
      get(id: string): Observable<FileData>
      {
        let url = this.getUrl("file/" + id);
        return this.http.get<FileData>(url);
      }

      getSharedFile(id: string): Observable<FileData>{
        let url = this.getUrl("file/shared/" + id);
        return this.http.get<FileData>(url);
      }

      put(item: FileData): Observable<FileData>{
        let url = this.getUrl("file/" + item.id);
        return this.http.put<FileData>(url, item);
      }

      share(item: FileData, email: string): Observable<FileData>{
        let url = this.getUrl("file/shared/" + email);
        return this.http.put<FileData>(url, item);
      }

      createShortLink(item: FileData): Observable<ShortLink>{
        let url = this.getUrl("shortlink/" + item.id);
        return this.http.post<ShortLink>(url, item);
      }

      deleteShortLink(item: FileData): Observable<boolean>{
        let url = this.getUrl("shortlink/" + item.shortLink);
        return this.http.delete<boolean>(url, { body : item});
      }

      delete(item: FileData): Observable<FileData>{
        let url = this.getUrl("file/" + item.id);
        return this.http.delete<FileData>(url, { body: item });
      }

      refuseShared(item: FileData): Observable<FileData>{
        let url = this.getUrl("file/shared/" + item.id);
        return this.http.delete<FileData>(url, { body: item});
      }

      download(id: string): Observable<Blob>
      {
        let url = this.getUrl("file/download/" + id);
        return this.http.get(url, {responseType: "blob"});
      }

      getShared(
          pageIndex: number,
          pageSize: number,
          sortColumn: string,
          sortOrder: string,
          filterColumn: string | null,
          filterQuery: string | null
        ): Observable<ApiResult<FileData>> {
          let url = this.getUrl("file/shared");
  
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
  
          return this.http.get<ApiResult<FileData>>(url, { params })
        }
  }