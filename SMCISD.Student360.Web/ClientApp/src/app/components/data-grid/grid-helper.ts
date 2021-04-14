import { Grid, GridHeader } from "./data-grid.component";

  export function calculateOrderByForRequest(grid: Grid) {
    var result = [];

    var sortedHeaders = grid.headers.slice().sort((a, b) => {
      if (a.orderNumber > b.orderNumber)
        return 1;
      if (b.orderNumber > a.orderNumber)
        return -1;

      return 0;
    });

    sortedHeaders.forEach(header => {
      if (header.order !== undefined) {
        result.push({ column: header.columnName, direction: header.order ? 'descending' : 'ascending' });
      }
    });

    return result;
  }

  export function camelCaseToPascalCase(camelCaseWord: string) {
    return camelCaseWord.charAt(0).toUpperCase() + camelCaseWord.slice(1);
  }

  export function camelCaseToSentence(camelCaseWord: string) {
    var result = camelCaseWord.replace(/([A-Z])/g, " $1");
    return camelCaseToPascalCase(result);
  }

  export function calculateHeaders(grid: Grid): GridHeader[] {
    var headers = [];
    grid.columns.forEach(col => {
      headers.push({ name: camelCaseToSentence(col), order: undefined, columnName: camelCaseToPascalCase(col), orderNumber: 0 });
    });
    return headers;
  }

