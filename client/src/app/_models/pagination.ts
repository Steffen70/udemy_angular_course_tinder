export interface Pagination {
    pageNumber: number;
    itemsPerPage: number;
    totalItems: number;
    totalPages: number;
    gender: string;
}

export class PaginatedResult<T> {
    result: T;
    pagiantion: Pagination;
}