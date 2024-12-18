window.LocalVenue = {
    Shaking: {
        normalHorizontalShake: function (Id) {
            let element = document.getElementById(Id);
            element.classList.add("shake-horizontal");

            setTimeout(() => {
                element.classList.remove('shake-horizontal');
            }, 300);
        },
        errorHorizontalShake: function (Id) {
            let element = document.getElementById(Id);
            element.classList.add("shake-horizontal");
            element.classList.add("transition-03")
            element.classList.add("box-shadow-red")

            setTimeout(() => {
                element.classList.remove('shake-horizontal');
            }, 300);

            setTimeout(() => {
                element.classList.remove("box-shadow-red")
            }, 2000)

            setTimeout(() => {
                element.classList.remove("transition-03")
            }, 2300)
        },
    },
    Draggable: {
        disableArrowAnimation: function () {
            const elements = document.querySelectorAll('.accordion-button');
            
            
            elements.forEach(element => {
                element.classList.add('no-transition');
            });
            
            setTimeout(() => {
                elements.forEach(element => {
                    element.classList.remove('no-transition');
                });
            }, 300);
        },
        toggleFadeAnimation: function (elementId) {
            let element = document.getElementById(elementId);
            let body = element.firstChild.lastChild;
            body.childNodes.forEach(child => {
                child.classList.toggle('FadeIn');
            });
        }
    },
    Bootstrap: {
        ModalShow: function (elementId) {
            let modal = document.getElementById(elementId);
            bootstrap?.Modal?.getOrCreateInstance(modal)?.show();
        },
        ModalClose: function (elementId) {
            let modal = document.getElementById(elementId);
            bootstrap?.Modal?.getOrCreateInstance(modal)?.hide();
        },
        ModalBindOnClose: function (elementId, dotNetHelper) {
            let modal = document.getElementById(elementId);
            modal.addEventListener('hidden.bs.modal', function () {
                dotNetHelper.invokeMethodAsync('BsHiddenModal');
            });
        },
        MakeIndeterminate: function (elementId) {
            let element = document.getElementById(elementId);
            element.indeterminate = true;
        },
        RemoveIndeterminate: function (elementId) {
            let element = document.getElementById(elementId);
            element.indeterminate = false;
        }
    },
    RemoveItem: {
        RemoveListItem: function (elementId) {
            let element = document.getElementById(elementId);

            let computedStyle = getComputedStyle(element);
            let paddingBottom = parseInt(computedStyle.paddingBottom, 10);
            let height = element.offsetHeight + paddingBottom;
            
            document.documentElement.style.setProperty('--element-height', `${height }px`);
            element.classList.add('remove-list-item');
        },
        StateChangeAfterRemoveListItem: function (elementId) {
            let element = document.getElementById(elementId);
            if (element) {
                element.classList.remove('remove-list-item');
            }
        }
    },
    TabList: {
        init: function () {
            Tablist.initAll();
        },
    },
    Collapse: {
        init: function (id, toggle, arrowId) {
            let element = document.getElementById(id);
            if (element != null) {
                bootstrap.Collapse.getOrCreateInstance(element, {toggle: toggle} );
            }
            if (!toggle && arrowId != null) {
                let arrow = document.getElementById(arrowId);
                arrow.classList.add('rotateDown');
            }
        },
        open: function (id, arrowId) {
            let element = document.getElementById(id);
            if (element != null) {
                if (arrowId) {
                    document.getElementById(arrowId).classList.add('rotateDown')
                }
                bootstrap.Collapse.getOrCreateInstance(element).show();
            }
        },
        hide: function (id, arrowId) {
            let element = document.getElementById(id);
            if (element != null) {
                if (arrowId) {
                    document.getElementById(arrowId).classList.remove('rotateDown')
                }
                bootstrap.Collapse.getOrCreateInstance(element).hide();
            }
        },
        toggle: function (id, arrowId) {
            let element = document.getElementById(id);
            if (element != null) {
                if (arrowId) {
                    let _isShown = bootstrap.Collapse.getOrCreateInstance(element)._isShown();
                    if (_isShown) {
                        document.getElementById(arrowId).classList.add('rotateDown')
                    } else {
                        document.getElementById(arrowId).classList.remove('rotateDown')
                    }
                }
                bootstrap.Collapse.getOrCreateInstance(element).toggle();
            }
        }
    },
    NavItems: {
        init: function () {
            const list = document.querySelectorAll(".nav-item");
            const currentPath = window.location.pathname;
            const activePath = currentPath.split("/")[1];
            
            list.forEach((item) => {
                const link = item.querySelector(".nav-item-link");

                // Get the href attribute value
                const href = link.getAttribute("href");
                
                if (href === activePath) {
                    item.classList.add("active");
                    }
                item.addEventListener("click", (e) => {
                    if (item.classList.contains("active")) {
                        e.preventDefault();
                    }
                    list.forEach((item) => {
                        item.classList.remove("active");
                    });
                    item.classList.add("active");
                });
            });

            const topMenu = document.getElementById('top-menu');
            let lastScrollTop = 0;
            
            window.addEventListener('scroll', function() {
                let scrollTop = window.pageYOffset || document.documentElement.scrollTop;
                if (scrollTop > lastScrollTop) {
                    // Scrolling down
                    topMenu.classList.add('hidden');
                    topMenu.classList.remove('visible');
                } else {
                    // Scrolling up
                    topMenu.classList.add('visible');
                    topMenu.classList.remove('hidden');
                }
                lastScrollTop = scrollTop <= 0 ? 0 : scrollTop; // For Mobile or negative scrolling
            }, false);
            
            document.addEventListener('mousemove', function(event) {
                if (event.clientY < 75) { // Adjust based on the height of your nav
                    topMenu.classList.add('visible');
                    topMenu.classList.remove('hidden');
                }
            });
        }
    },
    Input: {
        SetReadOnlyValue: function (elementId, readyOnly) {
            let element = document.getElementById(elementId);
            element.readOnly = readyOnly
        }
    },
    ClassList: {
        Toggle: function (elementId, className) {
            let element = document.getElementById(elementId);
            element.classList.toggle(className);
        },
        Add: function (elementId, className) {
            debugger;
            let element = document.getElementById(elementId);
            element.classList.add(className);
        },
        Remove: function (elementId, className) {
            let element = document.getElementById(elementId);
            element.classList.remove(className);
        },
        AddBodyClass: function (className) {
            document.body.classList.add(className); 
        },
        RemoveBodyClass: function (className) {
            document.body.classList.remove(className);
        },
    },
}