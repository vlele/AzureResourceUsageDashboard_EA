var app = angular.module('app', ['ngRoute', 'ngCookies']);
app.config(['$routeProvider', function ($routeProvider) {

    //dashboard pulls usage data grouped by subscription
    $routeProvider.when('/dash/:monthId?', {
        templateUrl: '/angular/templates/dashboard.html',
        controller: 'dashboardCtrl'
    });

    //dashboard pulls usage data grouped by account
    $routeProvider.when('/byAccount/:monthId?', {
        templateUrl: '/angular/templates/byAccount.html',
        controller: 'accountsCtrl'
    });

    //dashboard pulls usage data grouped by Azure Services
    $routeProvider.when('/byService/:monthId?', {
        templateUrl: '/angular/templates/byServices.html',
        controller: 'servicesCtrl'
    });
    $routeProvider.otherwise({
        redirectTo: '/dash'
    });
}]);

app.run();