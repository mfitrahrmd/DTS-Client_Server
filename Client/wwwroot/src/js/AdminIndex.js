import axios from "axios";
import Modal from "./components/modal/base";

const api = axios.create({
  baseURL: "http://localhost:5025/api",
  timeout: 5000,
  headers: {
    "Content-Type": "application/json",
    Accept: "application/json",
  },
});

$("#modal__base").on("hidden.bs.modal", () => {
  $("#modal__base .modal-header").siblings().remove();
});

function showBaseModal(title, modalBody, modalFooter) {
  const modal = bootstrap.Modal.getOrCreateInstance($("#modal__base"));
  if (title) $("#modal__base .modal-header .modal-title").text(title);
  if (modalBody) $("#modal__base .modal-content").append(modalBody);
  if (modalFooter) $("#modal__base .modal-content").append(modalFooter);
  modal.show();
}

$("#btn__insertEmployee").click(() => {
  showBaseModal(
    "Insert Employee",
    $(`
        <div class="card">
            <div class="card-content">
                <div class="card-body">
                    <form class="form" method="post">
                        <div class="row">
                            <div class="col-md-6 col-12">
                                <div class="form-group">
                                    <label for="nik-column">Nik</label>
                                    <input type="text" id="nik-column" class="form-control"
                                           placeholder="Nik" name="nik">
                                </div>
                            </div>
                            <div class="col-md-6 col-12">
                                <div class="form-group">
                                    <label for="firstName-column">First Name</label>
                                    <input type="text" id="firstName-column" class="form-control"
                                           placeholder="First Name" name="firstName">
                                </div>
                            </div>
                            <div class="col-md-6 col-12">
                                <div class="form-group">
                                    <label for="lastName-column">Last Name</label>
                                    <input type="text" id="lastName-column" class="form-control"
                                           placeholder="Last Name" name="lastName">
                                </div>
                            </div>
                            <div class="col-md-6 col-12">
                                <div class="form-group">
                                    <label for="birthdate-column">Birth Date</label>
                                    <input type="date" id="birthdate-column" class="form-control"
                                           placeholder="Birth Date" name="birthdate">
                                </div>
                            </div>
                            <div class="col-md-6 col-12">
                                <div class="form-group">
                                    <label for="gender-column">Gender</label>
                                    <select class="form-select" id="gender-column" name="gender">
                                        <option value="L">L</option>
                                        <option value="P">P</option>
                                    </select>
                                </div>
                            </div>
                            <div class="col-md-6 col-12">
                                <div class="form-group">
                                    <label for="hiringDate-column">Hiring Date</label>
                                    <input type="date" id="hiringDate-column" class="form-control"
                                           placeholder="Hiring Date" name="hiringDate">
                                </div>
                            </div>
                            <div class="col-md-6 col-12">
                                <div class="form-group">
                                    <label for="email-column">Email</label>
                                    <input type="email" id="email-column" class="form-control"
                                           name="email" placeholder="Email">
                                </div>
                            </div>
                            <div class="col-md-6 col-12">
                                <div class="form-group">
                                    <label for="phoneNumber-column">Phone Number</label>
                                    <input type="text" id="phoneNumber-column" class="form-control"
                                           name="phoneNumber" placeholder="Phone Number">
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-light-secondary"
                                    data-bs-dismiss="modal">
                                <i class="bx bx-x d-block d-sm-none"></i>
                                <span class="d-none d-sm-block">Close</span>
                            </button>
                            <button type="submit" class="btn btn-primary ml-1">
                                <i class="bx bx-check d-block d-sm-none"></i>
                                <span class="d-none d-sm-block">Insert</span>
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    `)
  );
});

function populateTableEmployees() {
  api
    .get("/employees/master")
    .then((res) => {
      $("table#table__employees tbody").html(
        res.data.map((employee) =>
          $(`
            <tr>
                <th scope="col">${employee.nik}</th>
                <td>${employee.firstName} ${employee.lastName}</td>
                <td>${new Date(employee.birthdate).toLocaleDateString()}</td>
                <td>${employee.gender}</td>
                <td>${employee.email}</td>
                <td class="d-flex justify-content-center align-items-center gap-2">
                    <a href="#"><i class="bi bi-trash3-fill"></i></a>
                    <a href="#"><i class="bi bi-three-dots"></i></a>
                </td>
            </tr>
        `)
        )
      );
    })
    .catch((err) => {
      console.log(err);
    });
}

populateTableEmployees();

$("#modal__insertEmployee form").submit((e) => {
  const loadingModal = bootstrap.Modal.getOrCreateInstance($("#loading"));

  loadingModal.show();

  const formData = new FormData(e.target);

  const data = Object.fromEntries(formData.entries());

  console.log(data);

  api
    .post("/employees", data)
    .then(() => {
      e.target.reset();
      populateTableEmployees();
    })
    .catch((err) => {
      console.log(err);
    })
    .finally(() => {
      setTimeout(() => {
        loadingModal.hide();
      }, 500);
    });

  return false;
});
