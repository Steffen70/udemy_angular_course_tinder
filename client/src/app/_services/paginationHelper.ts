import { HttpClient } from "@angular/common/http";
import { map } from "rxjs/operators";
import { PaginatedResult } from "../_models/pagination";
import { PaginationParams } from "../_models/paginationParams";

export function getPaginatiedResult<T>(url: string, http: HttpClient, paginationParams: PaginationParams) {
    const paginatedResult = new PaginatedResult<T>();
    let params = paginationParams.getPaginationHeaders();

    return http.get<T>(url, { observe: 'response', params }).pipe(
        map(response => {
            paginatedResult.result = response.body
            var paginationHeaders = response.headers.get('Pagination');
            if (paginationHeaders != null)
                paginatedResult.pagiantion = JSON.parse(paginationHeaders);
            return paginatedResult;
        })
    );
}