async function createAsync() {

    var form = document.getElementById('Tournaments');

    form.addEventListener('submit', async function (event) {

        event.preventDefault();

        var data = new FormData(form);
        var name = data.get('Name');

        var entryFee = parseFloat(data.get('EntryFee'));
        var startDate = data.get('StartDate');

        try {
            const response = await createTournamentAsync(name, entryFee, startDate);
            const content = await response.json();
            const errorMessage = GetErrorMessage(response.status, content);

            if (errorMessage == null) {
                document.cookie = content.id;
                window.location.href = "/Index";
            }
            else {
                alert(errorMessage);
            }
        } catch (error) {
            alert(error);
        }
    });
}