var app = angular.module('app');
app.controller('accountsCtrl', ['$scope', '$http', 'chartService', '$routeParams', function ($scope, $http, chartService, $routeParams) {

    $scope._chartService = chartService;

    $scope.initGraph = function () {

        $scope.monthId = $routeParams.monthId !== undefined ? $routeParams.monthId : ''

        //make server call to get data (summary)
        $http({
            method: 'GET',
            url: '/data/SpendingByAccount'
        }).then(function successCallback(response) {
            var data = {};
            data.title = 'Cost By Account Name',
            data.data = response.data
            data.series = response.data.data;
            chartService.drawPieChart('container1', data);
        });

        //make server call to get data (daily break-up)
        $http({
            method: 'GET',
            url: '/data/SpendingByAccountDaily?monthId=' + $scope.monthId
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