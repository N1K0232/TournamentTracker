function tournamentDetail(language)
{
    Alpine.data("tournaments", () => ({
        tournament: {
            id: '',
            name: '',
            entryFee: 0.0,
            startsAt: '',
            endsAt: ''
        },
        isBusy: false,
        errorMessage: '',

        getTournament: async function (id)
        {
            this.isBusy = true;

            try
            {
                const response = await getTournamentAsync(id, language);
                const content = await response.json();

                this.errorMessage = GetErrorMessage(response.status, content);
                if (this.errorMessage == null)
                {
                    this.tournament = content;
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

        edit() {
            window.location.href = `/Tournaments/Edit/${this.tournament.id}`;
        },

        remove() {
            window.location.href = `/Tournaments/Delete/${this.tournament.id}`;
        }
    }));
}

async function getTournamentAsync(id, language)
{
    const response = await fetch(`/api/tournaments/${id}`, {
        method: "GET",
        headers: {
            "Accept-Language": language
        }
    });

    return response;
}