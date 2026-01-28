window.renderDashboardCharts = (moodData, trendLabels, trendValues, wordLabels, wordValues) => {
    //helper to destroy old charts if they exist
    const destroyChart = (id) => {
        let chart = Chart.getChart(id);
        if (chart) chart.destroy();
    };

    destroyChart('trendChart');
    destroyChart('wordsBarChart');
    destroyChart('moodPieChart');

    //only rendering if we have data
    if (trendLabels.length > 0) {
        new Chart(document.getElementById('trendChart'), {
            type: 'line',
            data: {
                labels: trendLabels,
                datasets: [{ data: trendValues, borderColor: '#3b82f6', tension: 0.4, fill: false }]
            },
            options: { plugins: { legend: { display: false } }, maintainAspectRatio: false }
        });
    }

    if (wordLabels.length > 0) {
        new Chart(document.getElementById('wordsBarChart'), {
            type: 'bar',
            data: {
                labels: wordLabels,
                datasets: [{ data: wordValues, backgroundColor: '#3b82f6' }]
            },
            options: { plugins: { legend: { display: false } }, maintainAspectRatio: false }
        });
    }

    if (Object.keys(moodData).length > 0) {
        new Chart(document.getElementById('moodPieChart'), {
            type: 'pie',
            data: {
                labels: Object.keys(moodData),
                datasets: [{ data: Object.values(moodData), backgroundColor: ['#10b981', '#f59e0b', '#ef4444', '#3b82f6'] }]
            },
            options: { maintainAspectRatio: false }
        });
    }
};