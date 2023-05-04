// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
class APIService {
    baseUrl = "https://api.spotify.com/v1";
    
    async getRequest(url) {
        let result;
        
        try {
            result = await $.ajax({
                url,
                type: "GET",
                headers: {
                    Authorization: `${localStorage.getItem("token_type")} ${localStorage.getItem("access_token")}`
                }
            })
        } catch (err) {
            if (err.status === 401) {
                await RefreshToken()
            }
            
            throw err;
        }
        
        return result;
    }
    
    async accessToken(code) {
        let result;
        
        try {
            result = await $.ajax({
                url: "/api/Auth/AccessToken",
                type: "POST",
                data: JSON.stringify({
                    Code: code
                }),
                contentType: "application/json"
            })
        } catch (err) {
            console.log(err)
        }
        
        return result;
    }
    
    async refreshToken(refreshToken) {
        let result;
        
        try {
            result = await $.ajax({
                url: "/api/Auth/RefreshToken",
                type: "POST",
                data: JSON.stringify({
                    RefreshToken: refreshToken
                }),
                contentType: "application/json"
            })
        } catch (err) {
            console.log(err)
        }
        
        return result;
    }
}

// Components
function PlaylistCard(id, title, description, imageUrl) {
    return $(`
        <div id="${id}" class="col" style="cursor: pointer;">
            <div class="card">
              <img class="card-img-top" src="${imageUrl}" alt="Card image cap">
              <div class="card-body">
                <p class="card-title card__title text-truncate">${title}</p>
                <p class="card-text card__description text-truncate">${description}</p>
              </div>
            </div>
        </div>
    `)
}

function SongCard(name, duration, imageUrl, artistNames, externalUrl, previewUrl) {
    return $(`
        <div class="col">
            <div class="card mb-3">
              <div class="row g-0">
                <div class="col-md-4">
                  <img src="${imageUrl}" class="img-fluid rounded-start" alt="...">
                </div>
                <div class="col-md-8">
                  <div class="card-body">
                    <div class="position-absolute p-4 top-0 end-0">
                        <a href="${externalUrl}" target="_blank" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Open in spotify"><i class="bi bi-box-arrow-up-right"></i></a>
                    </div>
                    <h5 class="card-title">${name}</h5>
                    <p class="card-text">${artistNames.join(" , ")}</p>
                    <p class="card-text"><small class="text-muted">${new Date(duration).toISOString().slice(14, 19)}</small></p>
                    ${previewUrl ? `<figure>
                        <figcaption>Listen to the ${name} :</figcaption>
                        <audio id="previewSongPlayer"
                            controls
                            src="${previewUrl}">
                                <a href="${previewUrl}">
                                    Download audio
                                </a>
                        </audio>
                    </figure>` : `<p class="card-text">Preview not available</p>`}
                  </div>
                </div>
              </div>
            </div>
        </div>
    `)
}

function ButtonAction(text, cb) {
    return $(`
        <div class="col col-12 d-flex justify-content-center">
            <button type="button" class="btn btn-outline-dark">${text}</button>
        </div>
    `).on("click", cb);
}

function PlaylistCarousel(items) {
    const carousel = $(`
    <div id="carouselExampleCaptions" class="carousel slide" data-bs-ride="carousel">
  <div class="carousel-indicators">
  ${items.map((item, i) => {
      return `
          <button type="button" data-bs-target="#carouselExampleCaptions" data-bs-slide-to="${i}" class="${i === 0 ? "active" : ""}" aria-current="${i === 0 ? "true" : ""}" aria-label="Slide ${i}"></button>
      `
    })}
  </div>
  <div class="carousel-inner">
    
    ${items.map((item, i) => {
        return `
            <div class="carousel-item ${i === 0 ? "active" : ""}">
              <img src="${item.images[0].url}" class="d-block w-100" alt="${item.name}">
              <div class="carousel-caption d-none d-md-flex flex-column justify-content-center align-items-center w-75 position-absolute top-50 start-50 translate-middle">
                <h5>${item.name}</h5>
                <p>${item.description}</p>
              </div>
            </div>
        `
    })}
    
  </div>
  <button class="carousel-control-prev" type="button" data-bs-target="#carouselExampleCaptions" data-bs-slide="prev">
    <span class="carousel-control-prev-icon" aria-hidden="true"></span>
    <span class="visually-hidden">Previous</span>
  </button>
  <button class="carousel-control-next" type="button" data-bs-target="#carouselExampleCaptions" data-bs-slide="next">
    <span class="carousel-control-next-icon" aria-hidden="true"></span>
    <span class="visually-hidden">Next</span>
  </button>
</div>
    `)
    
    return carousel;
}

const apiService = new APIService();

function SaveAuthToStorage(auth) {
    Object.entries(auth).forEach(([key, value]) => {
        localStorage.setItem(key, value)
    })
}

async function RefreshToken() {
    const res = await apiService.refreshToken(localStorage.getItem("refresh_token"))
    
    SaveAuthToStorage(res)
}

// Main program
(async function () {
    const params = new Proxy(new URLSearchParams(window.location.search), {
        get: (sp, prop) => sp.get(prop)
    })

    if (params.code) {
        let res = await apiService.accessToken(params.code)

        SaveAuthToStorage(res)

        window.open(window.location.origin, "_self")
    }

    if (!localStorage.getItem("access_token")) return;

    $("#btn__login").remove()
    
    $("#exampleModal").on("hide.bs.modal", function () {
        $("audio#previewSongPlayer").trigger("pause")
    })
    
    function detailPlaylist(id) {
        const playlistUrl = `https://api.spotify.com/v1/playlists/${id}`;
        
        return async function () {
            const myModal = bootstrap.Modal.getOrCreateInstance($("#exampleModal"));

            const { name, description, tracks, type, external_urls, followers, owner } = await apiService.getRequest(playlistUrl)
            $("#exampleModalLabel").text(name);
            
            $(".tab-content #info").html("").append($(`
                <div class="d-flex flex-column position-relative py-4">
                    <div class="position-absolute p-4 top-0 end-0">
                        <a href="${external_urls.spotify}" target="_blank" data-bs-toggle="tooltip" data-bs-placement="bottom" title="Open in spotify"><i class="bi bi-box-arrow-up-right"></i></a>
                    </div>
                    <div class="d-flex flex-column">
                        <p>${type}</p>
                        <h3 class="text-description">${description}</h3>
                        <div class="d-flex flex-row">
                            <small>${owner.display_name}</small>
                            <small> • </small>
                            <small>${followers.total} likes</small>
                            <small> • </small>
                            <small>${tracks.total} songs</small>
                        </div>
                    </div>
                </div>
            `))

            $(".tab-content #songs").html("").append(tracks.items.map(item => {
                return SongCard(item.track.name, item.track.duration_ms, item.track.album.images[0].url, item.track.artists.map(artist => artist.name), item.track.external_urls.spotify, item.track.preview_url)
            }));

            myModal.toggle();
        }
    }
    
    async function showPlaylists() {
        const myPlaylistsUrl = "https://api.spotify.com/v1/me/playlists";
        const featuredPlaylistsUrl = "https://api.spotify.com/v1/browse/featured-playlists";
        
        const morePlaylists = async function (url) {
            const { items: playlists, next } = await apiService.getRequest(url);
            
            playlists.forEach(playlist => {
                $("#container__playlists").append(PlaylistCard("card__playlist", playlist.name, playlist.description, playlist.images[0].url).on("click", detailPlaylist(playlist.id)));
            });
            
            if (next) {
                $("#container__playlists").append(ButtonAction("Load More",async function() {
                    this.remove();
                    await morePlaylists(next)
                }))
            }
        }
        
        const featuredPlaylists = async function(url) {
            const { playlists } = await apiService.getRequest(url);
            
            $("#container__playlists").prepend($(`
                <div id="carousel__playlists" class="col col-6 text-center"></div>
            `).append([
                PlaylistCarousel(playlists.items),
                $(`<h3>Featured Playlists</h3>`)
            ]));
        }
        
        await morePlaylists(myPlaylistsUrl);
        await featuredPlaylists(featuredPlaylistsUrl);
    }
    
    await showPlaylists();
})()