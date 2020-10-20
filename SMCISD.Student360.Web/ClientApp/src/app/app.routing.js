"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var router_1 = require("@angular/router");
var home_component_1 = require("./home/home.component");
var login_component_1 = require("./login/login.component");
var counter_component_1 = require("./counter/counter.component");
var fetch_data_component_1 = require("./fetch-data/fetch-data.component");
var auth_guard_1 = require("./_infrastructure/auth.guard");
var landing_component_1 = require("./home/landing/landing.component");
var data_table_example_component_1 = require("./home/table-data/data-table-example.component");
var routes = [
    {
        path: 'home', component: home_component_1.HomeComponent, canActivate: [auth_guard_1.AuthGuard], children: [
            { path: '', component: landing_component_1.LandingComponent, canActivate: [auth_guard_1.AuthGuard] },
            { path: 'example', component: data_table_example_component_1.DataTableExampleComponent },
        ]
    },
    { path: 'login', component: login_component_1.LoginComponent },
    // otherwise redirect to home
    { path: '**', redirectTo: 'home' }
];
exports.appRoutingModule = router_1.RouterModule.forRoot(routes);
//# sourceMappingURL=app.routing.js.map
