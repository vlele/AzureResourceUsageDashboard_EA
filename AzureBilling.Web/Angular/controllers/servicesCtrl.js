var app = angular.module('app');
app.controller('servicesCtrl', ['$scope', '$http', 'chartService', '$routeParams', function ($scope, $http, chartService, $routeParams) {

    $scope._chartService = chartService;

    //initialize the graph
    $scope.initGraph = function () {

        $scope.monthId = $routeParams.monthId !== undefined ? $routeParams.monthId : ''

        //make server call to get data (summary)
        $http({
            method: 'GET',
            url: '/data/SpendingByService?monthId=' + $scope.monthId
        }).then(function successCallback(response) {
            var data1 = {};
            data1.title = 'Usage by Category';
            data1.data = response.data;
            $scope._chartService.drawPieChart('container1', data1);
        });

        //make server call to get data (daily break-up)
        $http({
            method: 'GET',
            url: '/data/SpendingByServiceDaily?monthId=' + $scope.monthId
        }).then(function successCallback(response) {
            var data = {};
            data.title = 'Daily Usage',
            data.categories = response.data.date;
            data.series = response.data.series;
            $scope._chartService.drawLineChart('container2', data);
        });
    };

    $scope.initGraph();
}]);