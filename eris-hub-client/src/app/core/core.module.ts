import { NgModule } from "@angular/core";
import { HttpClientModule } from "@angular/common/http";
import { NavbarComponent } from './components/navbar/navbar.component';

@NgModule({
    imports: [
        HttpClientModule
    ],
    providers: [
        
    ],
    declarations: [NavbarComponent],
    exports: [NavbarComponent]
})
export class CoreModule { }