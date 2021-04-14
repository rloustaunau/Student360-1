import { cpuUsage, title } from "process";

export function downloadCsv(csv: string, fileName: string) {
    var encodedUri = encodeURI(csv);
    var link = document.createElement("a");
    link.setAttribute("href", encodedUri);

    link.setAttribute("download", fileName + '_' + new Date().toLocaleDateString("en-US") + ".csv"); // There are 2 types of spaces that is why there is 2 replace methods
    document.body.appendChild(link);

    link.click();
}

export function generateCsv(data: any[], cols: string[], headers: string[], titleRow?: string[]): string {
    var rows = data.map(x => {
        return cols.map(col => {
            return x[col];
        });
    });

    var fileData = [];
    if (titleRow) {
        fileData.push(titleRow);
    }
    fileData.push(headers);
    fileData.push(...rows);

    return "data:text/csv;charset=utf-8," + fileData.map(x => x.join(',')).join('\n');
}
