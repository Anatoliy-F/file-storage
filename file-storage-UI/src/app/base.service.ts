import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { environment } from "src/environments/environment";

export abstract class BaseService<T> {

  constructor(
    protected http: HttpClient
  ) { }

  abstract getData(
    pageIndex: number,
    pageSize: number,
    sortColumn: string,
    sortOrder: string,
    filterColumn: string | null,
    filterQuery: string | null
  ) : Observable<ApiResult<T>>;

  abstract get(id: string): Observable<T>;
  abstract put(item: T): Observable<T>;

  //TODO: should i use it
  //abstract post(item:T): Observable<T>;
  
  protected getUrl(url: string){
    return environment.baseUrl + url;
  }
}

export interface ApiResult<T> {
  data: T[];
  pageIndex: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  sortColumn: string;
  sortOrder: string;
  filterColumn: string;
  filterQuery: string;
}
