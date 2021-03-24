export interface Pagination {
    pageNumber: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
}

export class PaginatedResult<T> {
    result: T;
    pagiantion: Pagination;
}