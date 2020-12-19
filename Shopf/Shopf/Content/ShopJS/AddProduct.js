<script src="~/Scripts/jquery-3.5.1.js"></script>
    <script src="~/Scripts/jquery-ui.js"></script>
    <script>
        $(function () {

            /* Preview selected image */

            function readURL(input) {
                if (input.files && input.files[0]) {
                    var reader = new FileReader();

                    reader.onload = function (e) {
                        $("img#imgpreview")
                            .attr("src", e.target.result)
                            .width(200)
                            .height(200);
                    }

                    reader.readAsDataURL(input.files[0]);
                }
            }

            $("#imageUpload").change(function () {
                readURL(this);
            });

            /*-----------------------------------------------------------*/

        });
    </script>