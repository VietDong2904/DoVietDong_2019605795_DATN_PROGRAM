export class QueryFilerModel {
    textSearch: string | undefined;
    pageSize!: number;
    pageNumber!: number;
    [key: string]: any;
  }  
  
export const ROLE_USER = 'USER';
export const PAGESIZE_MAX_VALUE = 999999;
export const LIST_SORT_TYPE = [
    { id: 0, name: 'Latest' },
    { id: 1, name: 'Prices increase' },
    { id: 2, name: 'Prices decrease' },
    { id: 3, name: 'View count' },
    { id: 4, name: 'Rating' },
    { id: 5, name: 'Name A -> Z' },
  ];
  export const QUERY_FILTER_DEFAULT: QueryFilerModel = {
    pageNumber: 1,
    pageSize: 99999,
    textSearch: undefined,
  };
  export const PAGE_SIZE_OPTION_DEFAULT = [5, 10, 20, 50];
