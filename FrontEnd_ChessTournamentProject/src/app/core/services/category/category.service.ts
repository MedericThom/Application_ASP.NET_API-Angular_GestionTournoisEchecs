import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { environment } from "../../../../environments/environment";
import { CategoryCreate } from "../../../models/category/category-create.interface";
import { Observable} from "rxjs";
import { CategoryResponse } from "../../../models/category/category-response.interface";

@Injectable({providedIn: 'root'})
export class CategoryService {
    private http = inject(HttpClient);

    private readonly apiUrl = `${environment.apiUrl}/category`;

    createCategory(category: CategoryCreate) : Observable<CategoryResponse>{
        return this.http.post<CategoryResponse>(this.apiUrl, category);
    }

    getAllCategories() : Observable<CategoryResponse[]>{
        return this.http.get<CategoryResponse[]>(this.apiUrl);
    }

    deleteCategory(id: number): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
}