function tournamentsList(language)
{
    Alpine.data("tournaments", () => ({
        tournaments: [
            {
                id: '',
                name: '',
                entryFee: 0.0,
                startsAt: '',
                endsAt: ''
            }
        ],
        isBusy: false,
        errorMessage: '',

        getList: async function ()
        {
            this.isBusy = true;

            try
            {
                const response = await getTournamentsAsync(language);
                const content = await response.json();

                this.errorMessage = GetErrorMessage(response.status, content);
                if (this.errorMessage == null)
                {
                    this.tournaments = content;
                }
                else
                {
                    alert(this.errorMessage);
                }
            }
            catch (error)
            {
                this.errorMessage = error.message;
            }
            finally
            {
                this.isBusy = false;
            }
        },

        showDetails(id)
        {
            window.location.href = `/TournamentDetails/${id}`;
        }
    }));
}

async function getTournamentsAsync(language)
{
    const response = await fetch('/api/tournaments', {
        method: "GET",
        headers: {
            "Accept-Language": language
        }
    });

    return response;
}