//部署:npm run build
function intDiv(i: number, j: number): number {
    return Math.floor(i / j)
}

class Sudoku9x9 {
    private bindTo: CS.Sudoku.Gameplay.Puzzle.Sudoku9x9;

    constructor(bindTo: CS.Sudoku.Gameplay.Puzzle.Sudoku9x9) {
        console.log("binding")
        this.bindTo = bindTo
        this.bindTo.jsDfs = (i: number, j: number) => this.dfs(i, j);
        this.bindTo.jsLog = () => this.log();
    }

    log(this: Sudoku9x9){
        console.log(this.bindTo)
    }

    dfs(this: Sudoku9x9, i: number, j: number): boolean {
        if (j >= 9) {
            if (i >= 8) return true;

            i++;
            j=0;
        }

        if (this.bindTo.get_Item(this.getIndex(i, j)) != 0) return this.dfs(i, j + 1)

        for (let num = 1; num <= 9; num++) {
            if (this.checkIsNumberAvailable(i, j, num)) {
                this.bindTo.set_Item(this.getIndex(i, j), num)
                if (this.dfs(i, j + 1)) return true
            }
        }

        this.bindTo.set_Item(this.getIndex(i, j), 0)
        return false
    }

    checkIsNumberAvailable(i: number, j: number, num: number): boolean {
        for (let k = 0; k < 9; k++) {
            if (this.bindTo.get_Item(this.getIndex(i, k)) == num) return false
            if (this.bindTo.get_Item(this.getIndex(k, j)) == num) return false
            if (this.bindTo.get_Item(this.getIndex(i - i % 3 + intDiv(k, 3), j - j % 3 + k % 3)) == num) return false
        }
        return true;
    }

    getIndex(this:Sudoku9x9, i: number, j: number): number {
        return i * this.bindTo.sideLength + j
    }
}

export function init(bindTo: CS.Sudoku.Gameplay.Puzzle.Sudoku9x9) {
    new Sudoku9x9(bindTo);
}